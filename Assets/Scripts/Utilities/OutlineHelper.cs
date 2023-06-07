using System.Collections;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Helper class to extends the Outline.cs functionalities
    /// </summary>
    public static class OutlineHelper
    {
        private const float OUTLINE_INIT_TIME = 0.001f;

        /// <summary>
        /// Initialize the outline to make sure its working correctly
        /// </summary>
        /// <param name="outline">Outline component reference</param>
        /// <returns></returns>
        public static IEnumerator InitializeOutline(Outline outline)
        {
            yield return new WaitForSeconds(OUTLINE_INIT_TIME);
            outline.enabled = true;
            yield return new WaitForSeconds(OUTLINE_INIT_TIME);
            outline.enabled = false;
        }
    }
}
