﻿ 
using System;
using System.Threading.Tasks;

namespace CUnity.Shared
{
    public class AppState
    {
        public event Action OnChange;

       // private UserProfile _userProfile { get; set; }

        public readonly string AppName = string.Empty;
        public readonly string AppShortName = string.Empty;
        public readonly string BreadCrumbHome = string.Empty;

        public AppState()
        {
            
            AppName ="CUnity";
            AppShortName = "CUnity";
            BreadCrumbHome = "Home";
        }

        //public bool IsNavOpen
        //{
        //    get
        //    {
        //        if (_userProfile == null)
        //            return true;

        //        return _userProfile.IsNavOpen;
        //    }
        //    set
        //    {
        //        _userProfile.IsNavOpen = value;
        //    }
        //}
        //public bool IsNavMinified { get; set; }

        //public async Task UpdateUserProfile()
        //{
        //    await _apiClient.SaveChanges();
        //}

        //public void ClearUserProfile()
        //{
        //    _userProfile = null;
        //}

        //public async Task<UserProfile> GetUserProfile()
        //{
        //    if (_userProfile == null)
        //        _userProfile = await _apiClient.GetUserProfile();

        //    return _userProfile;
        //}

        //public async Task UpdateUserProfileCount(int count)
        //{
        //    _userProfile.Count = count;
        //    await UpdateUserProfile();
        //    NotifyStateChanged();
        //}

        //public async Task<int> GetUserProfileCount()
        //{
        //    if (_userProfile == null)
        //    {
        //        _userProfile = await GetUserProfile();
        //        return _userProfile.Count;
        //    }

        //    return _userProfile.Count;
        //}

        //public async Task SaveLastVisitedUri(string uri)
        //{
        //    if (_userProfile == null)
        //    {
        //        _userProfile = await GetUserProfile();
        //    }
        //    else
        //    {
        //        _userProfile.LastPageVisited = uri;
        //        await UpdateUserProfile();

        //        NotifyStateChanged();
        //    }
        //}

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
