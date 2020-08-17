using System;

namespace EmailServer
{
    /// <summary>
    /// Id helpers
    /// </summary>
    public static class IdUtil
    {
        /// <summary>
        /// Convets strings into Guids as all our IDs are Guids
        /// </summary>
        public static Guid? GetId(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }
            else
            {
                return new Guid(id.Trim());
            }
        }
    }
}
