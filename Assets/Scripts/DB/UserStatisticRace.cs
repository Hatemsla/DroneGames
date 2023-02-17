﻿namespace DB
{
    public class UserStatisticRace
    {
        public int StatisticRaceId { get; set; }
        public int WinsCount { get; set; } 
        public int LosesCount { get; set; } 
        public int GamesCount { get; set; } 
        public int SecondsInGame { get; set; }

        public UserStatisticRace()
        {
        }

        public UserStatisticRace(int statisticFootballId, int winsCount, int losesCount, int gamesCount,
            int secondsInGame)
        {
            StatisticRaceId = statisticFootballId;
            WinsCount = winsCount;
            LosesCount = losesCount;
            GamesCount = gamesCount;
            SecondsInGame = secondsInGame;
        }
    }
}