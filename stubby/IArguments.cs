namespace stubby
{

    /// <summary>
    /// The options container class used by Stubby constructors.
    /// </summary>
    public interface IArguments
    {
        /// <summary>
        /// Port for admin portal.
        /// </summary>
        uint Admin { get; set; }

        /// <summary>
        /// If true, admin portal will not be started
        /// </summary>
        bool DisableAdmin { get; set; }

        /// <summary>
        /// Port for stubs portal.
        /// </summary>
        uint Stubs { get; set; }

        /// <summary>
        /// Port for stubs https portal.
        /// </summary>
        uint Tls { get; set; }

        /// <summary>
        /// If true, tls listener will not be started
        /// </summary>
        bool DisableTls { get; set; }

        /// <summary>
        /// Hostname at which to bind stubby.
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Data file location to pre-load endpoints.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Monitor supplied Data file for changes and reload endpoints if necessary.
        /// </summary>
        bool Watch { get; set; }

        /// <summary>
        /// Prevent stubby from loggin to the console.
        /// </summary>
        bool Mute { get; set; }
    }
}
