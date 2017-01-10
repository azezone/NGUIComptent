public enum MsgID
{
    CategoryDataReady = 1000,
    PhotoDataRefresh = 1001,
    CategoryPhotoLoad = 1002,

    ThemesDataReady = 1101,
    ThemePhotoDataRefresh = 1102,

    GetNextPageData = 2000,
    RefreshCtrBar = 2001,

    FolderIn = 3001,
    FolderOut = 3002,
    FolderPageCreated = 3003,
    LocalFolderDataReady = 3004,
    LocalPhotosInFolderDataReady = 3005,

    LeftMenu = 4000,
    LeftMenuChange = 4001,

    NetWorkNotAvaillable = 5000,
    NetWorkIsSlow = 5001,
    CheckRefreshPage = 5002,

    SettingNormal = 6000,
    SettingForceUpdate = 6001,
    SettingUpdating = 6002,
    SettingNormalFreshLabel = 6003,


    PlayNextImage = 7001,
    PlayLastImage = 7002,

    CheckUpdateInfo = 8001,
}