namespace FallGuysStats {
    public enum LevelType {
        Unknown,
        CreativeRace,
        CreativeSurvival,
        CreativeHunt,
        CreativeLogic,
        CreativeTeam,
        Race,
        Survival,
        Hunt,
        Logic,
        Team,
        Invisibeans,
        Final
    }
    
    public enum BestRecordType {
        Fastest,
        Longest,
        HighScore
    }
    
    public enum QualifyTier {
        Pink,
        Gold,
        Silver,
        Bronze
    }

    public enum Language {
        English,
        French,
        Korean,
        Japanese,
        SimplifiedChinese,
        TraditionalChinese
    }

    public enum StatType {
        Shows,
        Rounds,
        Levels,
    }
    
    public enum HashTypes { MD5, RIPEMD160, SHA1, SHA256, SHA384, SHA512 }
    
    public enum DWMWINDOWATTRIBUTE {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    public enum DWM_WINDOW_CORNER_PREFERENCE {
        DWMWCP_DEFAULT      = 0,
        DWMWCP_DONOTROUND   = 1,
        DWMWCP_ROUND        = 2,
        DWMWCP_ROUNDSMALL   = 3
    }
    
    public enum TaskbarPosition { Top, Bottom, Left, Right }
    
    public enum OnlineServiceTypes { None = -1, EpicGames = 0, Steam = 1 }
    
	/// <summary>
	/// Animation to display Toast
	/// </summary>
	public enum ToastAnimation {
		FADE = 1,
		SLIDE = 0
	}
    
    /// <summary>
    /// Way to closing Toast
    /// </summary>
    public enum ToastCloseStyle {
        ClickEntire,
        Button,
        ButtonAndClickEntire
    }
    
    /// <summary>
    /// Duration definition. Short is 2 seconds, long is 3 seconds
    /// </summary>
    public enum ToastDuration {
        VERY_SHORT = 2,
        SHORT = 4,
        MEDIUM = 6,
        LONG = 8,
        VERY_LONG = 10
    }
    
    /// <summary>
    /// Location of Toast. Only top right and bottom right supported
    /// </summary>
    public enum ToastPosition {
        TopRight,
        BottomRight
    }
    
    public enum ToastSound {
        Generic01,
        Generic02,
        Generic03,
        Generic04
    }
    
    public enum ToastTheme {
        Dark,
        Light,
        PrimaryLight,
        SuccessLight,
        WarningLight,
        ErrorLight,
        PrimaryDark,
        SuccessDark,
        WarningDark,
        ErrorDark,
        Custom
    }
}
