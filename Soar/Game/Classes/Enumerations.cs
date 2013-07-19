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

namespace Paradox.Game.Classes
{
    #region Enumerations

    //Some constants for Game Actions
    public enum InputActionMenu : byte { Up, Down, Left, Right, Enter, Exit, Select };
    public enum InputAction : byte { DecreaseSpeed = 0, IncreaseSpeed, Up, Down, Left, Right, LinearLeft, LinearRight, Fire1, Fire2, WeaponChange, IncreaseTurningSpeed, DecreaseTurningSpeed, Pause };
    public enum InputActionEnemy : byte { Forward = 0, Reverse, Stop };
    public enum InputActions : byte { Forward = 0, ForwardAway, Stop };
    public enum EnemyType : byte { SmallFighter, MediumFighter, LargeFighter };
    public enum WeaponType : byte { Bullets, Missles };
    public enum CollidableType : byte { None, PlayerBullet, EnemyBullet, FriendBullet, FriendMissle, FriendlyShip, EnemyShip, Asteroid, Dock, Station, EnemyBase, FriendBase };
    public enum AsteroidType : byte { Small = 0, Medium = 1, Large = 2 };
    public enum SoundType : byte { Player_Engine, Player_Hit, Player_Shoot, Friend_Shoot, Enemy_Shoot, Explosion, PlayerMissle, None };
    public enum SoundVoiceType : byte { Start, StationHit, Leaving, Lose, Win, Completed, Failed };
    public enum ExplosionType : byte { Big, Small };
    
    #endregion
}
