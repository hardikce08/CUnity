using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared
{
    public static class Constants
    {
        public const string LoginPath = "/login";
        public const string HomePath = "/";
        public const string TenantId = "523b271a-93b6-40a2-95ca-da7db06d70f1";
        public const string AppId = "7f64d0af-aa27-452c-888b-7e09b0223b11";
        public const string ClientSecret = "/XgcJRebFfxaNbzN8vTlEV7zflX5loiV6dCUfE/AfHc=";

    }

    public static class DefaultRoleNames
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Administrator = "Administrator";
        public const string User = "User";
    }

    public enum ProductNames
    {
        HouseHolding = 1,
        Optics = 2
    }
}
