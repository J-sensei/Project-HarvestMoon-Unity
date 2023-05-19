namespace GameDateTime
{
    /// <summary>
    /// Observal pattern, to get update from the game time manager
    /// </summary>
    public interface ITimeChecker
    {
        /// <summary>
        /// Get the game time when time is tick
        /// </summary>
        /// <param name="gameTime"></param>
        void ClockUpdate(GameTime gameTime);
        /// <summary>
        /// Call when a new day is start
        /// </summary>
        /// <param name="gameTime"></param>
        void NewDay(GameTime gameTime);
    }
}
