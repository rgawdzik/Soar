/*This code is managed under the Apache v2 license. 
To see an overview: 
http://www.tldrlegal.com/license/apache-license-2.0-(apache-2.0)

Author: Robert Gawdzik
www.github.com/rgawdzik/

THIS CODE HAS NO FORM OF ANY WARRANTY, AND IS CONSIDERED AS-IS.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
namespace Paradox.Game.Classes.Engines
{
    public class SoundEngine
    {
        #region Variables and Properties

        List<Sound> SoundList;
        List<SoundVoice> SoundVoiceList;

        List<SoundEffect[]> SoundVoiceEffects;
        List<SoundEffectInstance> SoundVoiceInstances;

        List<SoundEffectInstance> SoundInstances;
        SoundEffect[] SoundEffects { get; set; }


        SoundEffect[] VoiceChatterSounds { get; set; }
        List<SoundEffectInstance> VoiceInstances;

        SoundEffect[] _music { get; set; }
        SoundEffectInstance _musicInstance;
        int _selectedMusic;

        private Random _randVar;
        private TimeSpan _timeSincelastChatter;
        TimeSpan _timeChatterConst = new TimeSpan(0, 0, 35);

        Save _save;

        #endregion

        #region Constructor
        public SoundEngine(SoundEffect[] soundEffects, SoundEffect[] voiceChatterSounds, SoundEffect[] music, List<SoundEffect[]> soundVoiceEffects, Save save)
        {
            SoundEffects = soundEffects;
            SoundList = new List<Sound>();
            SoundInstances = new List<SoundEffectInstance>();
            _randVar = new Random();
            VoiceInstances = new List<SoundEffectInstance>();
            VoiceChatterSounds = voiceChatterSounds;

            SoundVoiceList = new List<SoundVoice>();
            SoundVoiceEffects = soundVoiceEffects;
            SoundVoiceInstances = new List<SoundEffectInstance>();
            _music = music;
            _selectedMusic = _randVar.Next(_music.Length);
            _save = save;
        }
        #endregion

        #region Update

        public void UpdateSound(Vector3 playerPosition, GameTime gameTime)
        {
            if (_save.Config.SoundEnabled)
            {
                for (int i = 0; i < SoundList.Count(); i++)
                {
                    SoundList[i].UpdateVolume(playerPosition); //Updates the sound relevant to the playerPosition.


                    if (SoundList[i].IsCreated && SoundInstances.Count < 16) //Only 16 Sound Instances can play at the same time.
                    {
                        if (SoundList[i].Volume > 0)
                        {
                            SoundInstances.Add(SoundEffects[(byte)SoundList[i].SoundType].CreateInstance()); //Adds the SoundInstance to the same position as the Sound in the two seperate arrays.
                            SoundList[i].IsCreated = false;
                            SoundInstances[i].IsLooped = SoundList[i].IsLooped;
                            SoundInstances[i].Pitch = SoundList[i].Pitch;
                            SoundInstances[i].Volume = SoundList[i].Volume;

                            SoundInstances[i].Play();
                        }
                        else //Remove this noise, if the player cannot hear it why play it?
                        {
                            SoundList.Remove(SoundList[i--]);
                        }
                    }
                    if (i > 0 && SoundInstances.Count < 16) //This gate makes sure that there is a soundinstance to begin with.
                    {
                        if (SoundInstances[i].State == SoundState.Paused || SoundInstances[i].State == SoundState.Stopped) //The sound has stopped, let's see if it was a problem with the loop.
                        {
                            if (SoundList[i].IsLooped) //A problem has occured, the sound state was not supposed to stop.
                            {
                                SoundInstances[i].IsLooped = SoundList[i].IsLooped;
                                SoundInstances[i].Play();
                            }
                            else //This soundtype was never supposed to loop.  This means it ended naturally.
                            {
                                SoundList.Remove(SoundList[i]);
                                SoundInstances.Remove(SoundInstances[i--]);
                            }
                        }
                    }
                    else if (i > 0)
                    {
                        if (SoundInstances[i - 1].State == SoundState.Paused || SoundInstances[i - 1].State == SoundState.Stopped) //The sound has stopped, let's see if it was a problem with the loop.
                        {
                            if (SoundList[i].IsLooped) //A problem has occured, the sound state was not supposed to stop.
                            {
                                SoundInstances[i - 1].IsLooped = SoundList[i].IsLooped;
                                SoundInstances[i].Play();
                            }
                            else //This soundtype was never supposed to loop.  This means it ended naturally.
                            {
                                SoundList.Remove(SoundList[i]);
                                SoundInstances.Remove(SoundInstances[i--]);
                            }
                        }
                    }
                }
            }

            ManageChatter(gameTime);
            ManageMusic();
            ManageSoundVoice();
        }

        #endregion

        #region Helper Methods

        public void ImportSound(List<Sound> newSounds)
        {
            SoundList.AddRange(newSounds);
        }

        public void ImportSound(Sound newSound)
        {
            SoundList.Add(newSound);
        }

        public void ImportSoundVoice(List<SoundVoice> soundVoice)
        {
            SoundVoiceList.AddRange(soundVoice);
        }
        /// <summary>
        /// Manages the voices, such as the battlestation.
        /// </summary>
        public void ManageSoundVoice()
        {
            if (_save.Config.VoiceEnabled)
            {
                for (int i = 0; i < SoundVoiceList.Count; i++)
                {
                    if (SoundVoiceList[i].IsCreated)
                    {
                        SoundVoiceList[i].IsCreated = false;
                        //Creates a sound Instance from the List of SoundEffect arrays, and takes a random voice from that array.
                        SoundVoiceInstances.Add(SoundVoiceEffects[(byte)SoundVoiceList[i].SoundVoiceType][_randVar.Next(SoundVoiceEffects[(byte)SoundVoiceList[i].SoundVoiceType].Count())].CreateInstance());
                        SoundVoiceInstances[i].Play();
                    }
                    //Makes sure that we are working with an existing sound. [gate]
                    if (i < SoundVoiceInstances.Count)
                    {
                        if (SoundVoiceInstances[i].State == SoundState.Paused || SoundVoiceInstances[i].State == SoundState.Stopped) //The sound has stopped, let's see if it was a problem with the loop.
                        {
                            if (SoundVoiceList[i].IsLooped) //A problem has occured, the sound state was not supposed to stop.
                            {
                                SoundVoiceInstances[i].IsLooped = SoundVoiceList[i].IsLooped;
                                SoundVoiceInstances[i].Play();
                            }
                            else //This soundtype was never supposed to loop.  This means it ended naturally.
                            {
                                SoundVoiceList.Remove(SoundVoiceList[i]);
                                SoundVoiceInstances.Remove(SoundVoiceInstances[i--]);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Manages the random Chatter.
        /// </summary>
        public void ManageChatter(GameTime gameTime)
        {
            if (_save.Config.VoiceEnabled)
            {
                _timeSincelastChatter += gameTime.ElapsedGameTime;

                if (_timeSincelastChatter >= _timeChatterConst)
                {
                    _timeSincelastChatter = TimeSpan.Zero;
                    SoundEffectInstance instance = VoiceChatterSounds[_randVar.Next(VoiceChatterSounds.Count())].CreateInstance();
                    instance.Play();
                    VoiceInstances.Add(instance);
                }

                for (int i = 0; i < VoiceInstances.Count; i++)
                {
                    if (VoiceInstances[i].State == SoundState.Paused || VoiceInstances[i].State == SoundState.Stopped)
                        VoiceInstances.Remove(VoiceInstances[i--]);
                }
            }
        }

        public void ManageMusic()
        {
            if (_save.Config.MusicEnabled)
            {
                if (_musicInstance == null) //There is no music.  Let's play some.
                {
                    if (_selectedMusic == _music.Count())
                        _selectedMusic = 0;
                    _musicInstance = _music[_selectedMusic++].CreateInstance();
                    _musicInstance.Volume = 0.3f;
                    _musicInstance.Play();
                }
                else if (_musicInstance.State == SoundState.Stopped || _musicInstance.State == SoundState.Paused)//A musicInstance exists already, and it stopped.
                {
                    if (!_save.Config.End)
                    {
                        if (_selectedMusic == _music.Count())
                            _selectedMusic = 0;
                        _musicInstance = _music[_selectedMusic++].CreateInstance();
                        _musicInstance.Volume = 0.3f;
                        _musicInstance.Play();
                    }
                }
            }
        }

        public void StopMusic()
        {
            if(_musicInstance != null)
                _musicInstance.Stop();
        }
        #endregion
    }
}
