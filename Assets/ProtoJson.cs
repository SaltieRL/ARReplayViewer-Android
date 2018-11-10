using System.Collections.Generic;

public class Score
{
    public int team0Score { get; set; }
    public int team1Score { get; set; }
}

public class PlayerId
{
    public string id { get; set; }
}

public class Goal
{
    public int frameNumber { get; set; }
    public PlayerId playerId { get; set; }
}

public class AttackerId
{
    public string id { get; set; }
}

public class VictimId
{
    public string id { get; set; }
}

public class Demo
{
    public int frameNumber { get; set; }
    public AttackerId attackerId { get; set; }
    public VictimId victimId { get; set; }
}

public class GameMetadata
{
    public string id { get; set; }
    public string name { get; set; }
    public string map { get; set; }
    public int version { get; set; }
    public string time { get; set; }
    public int frames { get; set; }
    public Score score { get; set; }
    public List<Goal> goals { get; set; }
    public List<Demo> demos { get; set; }
    public double length { get; set; }
    public string gameServerId { get; set; }
    public string serverName { get; set; }
    public string matchGuid { get; set; }
    public int teamSize { get; set; }
    public string playlist { get; set; }
}

public class Id
{
    public string id { get; set; }
}

public class CameraSettings
{
    public double stiffness { get; set; }
    public double height { get; set; }
    public double transitionSpeed { get; set; }
    public double pitch { get; set; }
    public double swivelSpeed { get; set; }
    public double fieldOfView { get; set; }
    public double distance { get; set; }
}

public class Loadout
{
    public int banner { get; set; }
    public int boost { get; set; }
    public int car { get; set; }
    public int goalExplosion { get; set; }
    public int skin { get; set; }
    public int trail { get; set; }
    public int wheels { get; set; }
    public int version { get; set; }
    public int topper { get; set; }
    public int antenna { get; set; }
    public int engineAudio { get; set; }
}

public class Boost
{
    public double boostUsage { get; set; }
    public int numSmallBoosts { get; set; }
    public int numLargeBoosts { get; set; }
    public double wastedCollection { get; set; }
    public double wastedUsage { get; set; }
    public double timeFullBoost { get; set; }
    public double timeLowBoost { get; set; }
    public double timeNoBoost { get; set; }
    public int numStolenBoosts { get; set; }
    public double averageBoostLevel { get; set; }
    public double wastedBig { get; set; }
    public double wastedSmall { get; set; }
}

public class Distance
{
    public double ballHitForward { get; set; }
    public double ballHitBackward { get; set; }
    public double timeClosestToBall { get; set; }
    public double timeFurthestFromBall { get; set; }
    public double timeCloseToBall { get; set; }
    public double timeClosestToTeamCenter { get; set; }
    public double timeFurthestFromTeamCenter { get; set; }
}

public class Possession
{
    public double possessionTime { get; set; }
    public int turnovers { get; set; }
    public int turnoversOnMyHalf { get; set; }
    public int turnoversOnTheirHalf { get; set; }
    public int wonTurnovers { get; set; }
}

public class PositionalTendencies
{
    public double timeOnGround { get; set; }
    public double timeLowInAir { get; set; }
    public double timeHighInAir { get; set; }
    public double timeInDefendingHalf { get; set; }
    public double timeInAttackingHalf { get; set; }
    public double timeInDefendingThird { get; set; }
    public double timeInNeutralThird { get; set; }
    public double timeInAttackingThird { get; set; }
    public double timeBehindBall { get; set; }
    public double timeInFrontBall { get; set; }
    public double timeNearWall { get; set; }
    public double timeInCorner { get; set; }
}

public class Averages
{
    public double averageSpeed { get; set; }
    public double averageHitDistance { get; set; }
    public double averageDistanceFromCenter { get; set; }
}

public class HitCounts
{
    public int totalHits { get; set; }
    public int totalPasses { get; set; }
    public int totalShots { get; set; }
    public int totalDribbles { get; set; }
    public int totalDribbleConts { get; set; }
    public int totalAerials { get; set; }
}

public class Controller
{
    public bool isKeyboard { get; set; }
    public double analogueSteeringInputPercent { get; set; }
    public double analogueThrottleInputPercent { get; set; }
}

public class Speed
{
    public double timeAtSlowSpeed { get; set; }
    public double timeAtSuperSonic { get; set; }
    public double timeAtBoostSpeed { get; set; }
}

public class RelativePositioning
{
    public double timeInFrontOfCenterOfMass { get; set; }
    public double timeBehindCenterOfMass { get; set; }
    public double timeMostForwardPlayer { get; set; }
    public double timeMostBackPlayer { get; set; }
    public double timeBetweenPlayers { get; set; }
}

public class Stats
{
    public Boost boost { get; set; }
    public Distance distance { get; set; }
    public Possession possession { get; set; }
    public PositionalTendencies positionalTendencies { get; set; }
    public Averages averages { get; set; }
    public HitCounts hitCounts { get; set; }
    public Controller controller { get; set; }
    public Speed speed { get; set; }
    public RelativePositioning relativePositioning { get; set; }
}

public class PartyLeader
{
    public string id { get; set; }
}

public class Player
{
    public Id id { get; set; }
    public string name { get; set; }
    public int titleId { get; set; }
    public int score { get; set; }
    public int goals { get; set; }
    public int assists { get; set; }
    public int saves { get; set; }
    public int shots { get; set; }
    public CameraSettings cameraSettings { get; set; }
    public Loadout loadout { get; set; }
    public int isOrange { get; set; }
    public Stats stats { get; set; }
    public PartyLeader partyLeader { get; set; }
    public bool isBot { get; set; }
    public double timeInGame { get; set; }
    public int firstFrameInGame { get; set; }
}

public class PlayerId2
{
    public string id { get; set; }
}

public class Possession2
{
    public double possessionTime { get; set; }
    public int turnovers { get; set; }
    public int turnoversOnMyHalf { get; set; }
    public int turnoversOnTheirHalf { get; set; }
    public int wonTurnovers { get; set; }
}

public class HitCounts2
{
    public int totalHits { get; set; }
    public int totalPasses { get; set; }
    public int totalShots { get; set; }
    public int totalDribbles { get; set; }
    public int totalDribbleConts { get; set; }
    public int totalAerials { get; set; }
}

public class PositionalTendencies2
{
    public double timeOnGround { get; set; }
    public double timeLowInAir { get; set; }
    public double timeHighInAir { get; set; }
    public double timeInDefendingHalf { get; set; }
    public double timeInAttackingHalf { get; set; }
    public double timeInDefendingThird { get; set; }
    public double timeInNeutralThird { get; set; }
    public double timeInAttackingThird { get; set; }
    public double timeBehindBall { get; set; }
    public double timeInFrontBall { get; set; }
    public double timeNearWall { get; set; }
    public double timeInCorner { get; set; }
}

public class CenterOfMass
{
    public PositionalTendencies2 positionalTendencies { get; set; }
    public double averageDistanceFromCenter { get; set; }
    public double averageMaxDistanceFromCenter { get; set; }
    public double timeClumped { get; set; }
    public double timeBoondocks { get; set; }
}

public class Stats2
{
    public Possession2 possession { get; set; }
    public HitCounts2 hitCounts { get; set; }
    public CenterOfMass centerOfMass { get; set; }
}

public class Team
{
    public List<PlayerId2> playerIds { get; set; }
    public int score { get; set; }
    public bool isOrange { get; set; }
    public Stats2 stats { get; set; }
}

public class PlayerId3
{
    public string id { get; set; }
}

public class BallData
{
    public double posX { get; set; }
    public double posY { get; set; }
    public double posZ { get; set; }
}

public class Hit
{
    public int frameNumber { get; set; }
    public PlayerId3 playerId { get; set; }
    public double collisionDistance { get; set; }
    public BallData ballData { get; set; }
    public double distance { get; set; }
    public double distanceToGoal { get; set; }
    public int nextHitFrameNumber { get; set; }
    public int goalNumber { get; set; }
    public bool isKickoff { get; set; }
    public int? previousHitFrameNumber { get; set; }
    public bool? aerial { get; set; }
    public bool? shot { get; set; }
    public bool? goal { get; set; }
    public bool? dribble { get; set; }
    public bool? dribbleContinuation { get; set; }
    public bool? pass { get; set; }
    public bool? passed { get; set; }
}

public class PositionalTendencies3
{
    public double timeOnGround { get; set; }
    public double timeLowInAir { get; set; }
    public double timeHighInAir { get; set; }
    public double timeInDefendingHalf { get; set; }
    public double timeInAttackingHalf { get; set; }
    public double timeInDefendingThird { get; set; }
    public double timeInNeutralThird { get; set; }
    public double timeInAttackingThird { get; set; }
    public double timeBehindBall { get; set; }
    public double timeInFrontBall { get; set; }
    public double timeNearWall { get; set; }
    public double timeInCorner { get; set; }
}

public class Averages2
{
    public double averageSpeed { get; set; }
}

public class BallStats
{
    public PositionalTendencies3 positionalTendencies { get; set; }
    public Averages2 averages { get; set; }
}

public class Kickoff
{
    public int startFrameNumber { get; set; }
    public int endFrameNumber { get; set; }
}

public class GameStats
{
    public List<Hit> hits { get; set; }
    public double neutralPossessionTime { get; set; }
    public BallStats ballStats { get; set; }
    public List<Kickoff> kickoffs { get; set; }
}

public class LeaderId
{
    public string id { get; set; }
}

public class Member
{
    public string id { get; set; }
}

public class Party
{
    public LeaderId leaderId { get; set; }
    public List<Member> members { get; set; }
}

public class Mutators
{
    public string ballType { get; set; }
    public int gameMutatorIndex { get; set; }
}

public class Proto
{
    public GameMetadata gameMetadata { get; set; }
    public List<Player> players { get; set; }
    public List<Team> teams { get; set; }
    public GameStats gameStats { get; set; }
    public List<Party> parties { get; set; }
    public int version { get; set; }
    public Mutators mutators { get; set; }
}