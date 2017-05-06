using System;

namespace SwarmSimFramework.SupportClasses
{
    public static class RandomNumber
    {
        private static Random randomNumber = new Random(13051995);

        public static int GetRandomInt(int min, int max)
        {
            return randomNumber.Next(min, max);
        }

        public static int GetRandomInt(int max)
        {
            return randomNumber.Next(max);
        }

        public static int GetRandomInt()
        {
            return randomNumber.Next();
        }

    }
}