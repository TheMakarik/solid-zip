namespace SolidZip.Localization;

public sealed class StrongTypedLocalizationManager(IUserJsonManager manager)
{
   public string AllDirectoriesMenuItem => Resources.Localization.AllDirectoriesMenuItem;
   public string AllFiles => Resources.Localization.AllFiles;
   public string ArchiveMenuItem => Resources.Localization.ArchiveMenuItem;
   public string CreateMenuItem => Resources.Localization.CreateMenuItem;
   public string DirectoryMenuItem => Resources.Localization.DirectoryMenuItem;
   public string EditMenuItem => Resources.Localization.EditMenuItem;
   public string FileMenuItem => Resources.Localization.FileMenuItem;
   public string ListMenuItem => Resources.Localization.ListMenuItem;
   public string RedoMenuItem => Resources.Localization.RedoMenuItem;
   public string RenameMenuItem => Resources.Localization.RenameMenuItem;
   public string SelectMenuItem => Resources.Localization.SelectMenuItem;
   public string ServicesMenuItem => Resources.Localization.ServicesMenuItem;
   public string TableMenuItems => Resources.Localization.TableMenuItems;
   public string UndoMenuItem => Resources.Localization.UndoMenuItem;
   public string ViewMenuItem => Resources.Localization.ViewMenuItem;
   public string UnauthorizedAccessText => Resources.Localization.UnauthorizedAccessText;
   public string ZipFile => Resources.Localization.ZipFile;
   public string LoadConfiguration => Resources.Localization.LoadConfiguration;
   public string LoadDependencies => Resources.Localization.LoadDependencies;
   public string LoadLogger => Resources.Localization.LoadLogger;
   public string LoadLua => Resources.Localization.LoadLua;
   public string LoadAppData => Resources.Localization.LoadAppData;
   public string Preparing => Resources.Localization.Preparing;
   public string Loading => Resources.Localization.Loading;
   public string ApplicationName => Resources.Localization.ApplicationName;
   public string Close => Resources.Localization.Close;
   public string SaveChanges => Resources.Localization.SaveChanges;
   
   public string NeedToSaveChanges => Resources.Localization.NeedToSaveChanges;
   public string Yes => Resources.Localization.Yes;
   public string No => Resources.Localization.No;
   public string DoNotClose => Resources.Localization.DoNotClose;
   public string Settings => Resources.Localization.Settings;
   public string Themes => Resources.Localization.Themes;
   public string GeneralSettings => Resources.Localization.GeneralSettings;
   public string Language => Resources.Localization.Language;
   public string EditTheme => Resources.Localization.EditTheme;

   public string LanguageSettings => Resources.Localization.LanguageSettings;
   public string LanguageSettingsTooltip => Resources.Localization.LanguageSettingsTooltip;
   public string ThemeSettings => Resources.Localization.ThemeSettings;
   public string ThemeSettingsTooltip => Resources.Localization.ThemeSettingsTooltip;
   public string ExplorerSettings => Resources.Localization.ExplorerSettings;
   public string ExplorerSettingsTooltip => Resources.Localization.ExplorerSettingsTooltip;
   public string FileDisplaySettings => Resources.Localization.FileDisplaySettings;
   public string FileDisplaySettingsTooltip => Resources.Localization.FileDisplaySettingsTooltip;

   public string LanguageTooltip => Resources.Localization.LanguageTooltip;
   public string SelectLanguageTooltip => Resources.Localization.SelectLanguageTooltip;
   public string ShowLuaConsole => Resources.Localization.ShowLuaConsole;
   public string ShowLuaConsoleTooltip => Resources.Localization.ShowLuaConsoleTooltip;
   public string ExplorerViewStyle => Resources.Localization.ExplorerViewStyle;
   public string ExplorerViewStyleTooltip => Resources.Localization.ExplorerViewStyleTooltip;
   public string SelectViewStyleTooltip => Resources.Localization.SelectViewStyleTooltip;
   public string ShowHiddenDirectories => Resources.Localization.ShowHiddenDirectories;
   public string ShowHiddenDirectoriesTooltip => Resources.Localization.ShowHiddenDirectoriesTooltip;
   public string FileSizeDisplay => Resources.Localization.FileSizeDisplay;
   public string FileSizeDisplayTooltip => Resources.Localization.FileSizeDisplayTooltip;
   public string SelectFileSizeDisplayTooltip => Resources.Localization.SelectFileSizeDisplayTooltip;
   public string AdditionalRootFiles => Resources.Localization.SampleFiles;
   public string AdditionalRootFilesTooltip => Resources.Localization.SampleFilesTooltip;
   public string SelectFileTooltip => Resources.Localization.SelectFileTooltip;
   public string Table => Resources.Localization.Table;
   public string Grid => Resources.Localization.Grid;
   public string List => Resources.Localization.List;
   public string Megabytes => Resources.Localization.Megabytes;
   public string Kilobytes => Resources.Localization.Kilobytes;
   public string Bytes => Resources.Localization.Bytes;
   public string ChangeTheme => Resources.Localization.ChangeTheme;
   public string ChangeThemeTooltip => Resources.Localization.ChangeThemeTooltip;
   public string Reset => Resources.Localization.Reset;
   public string ResetToDefaultsTooltip => Resources.Localization.ResetToDefaultsTooltip;
   public string SaveChangesTooltip => Resources.Localization.SaveChangesTooltip;
   public string Browse => Resources.Localization.Browse;
   
   [UsedImplicitly] public string AddDirectory => Resources.Localization.AddDirectory;
   [UsedImplicitly] public string RemoveDirectory => Resources.Localization.RemoveDirectory;

   public string LuaPlugins => Resources.Localization.LuaPlugins;
   public string SelectDirectory => Resources.Localization.SelectDirectory;
   public string ChangeLocalizationWarning => Resources.Localization.ChangeLocalizationWarning;
   public string ErrorOccured => Resources.Localization.ErrorOccured;
   public string ContactSupport => Resources.Localization.ContactSupport;
   public string ShowLogs => Resources.Localization.ShowLogs;
   public string SetArchiveName => Resources.Localization.SetArchiveName;
   public string SelectOutputFolder => Resources.Localization.SelectOutputFolder;
   public string CreateArchive => Resources.Localization.CreateArchive;
   public string UnexpectedErrorOccured => Resources.Localization.UnexpectedErrorText;
   public string NewZip => Resources.Localization.NewZip;
   public string Password =>  Resources.Localization.Password;
   public string Encryption => Resources.Localization.Encryption;
   public string SelectedFiles =>  Resources.Localization.SelectedFiles;
   public string NewDirectory => Resources.Localization.NewDirectory;
   public string InsertDirectoryName => Resources.Localization.InsertDirectoryName;
   public string DirectoryAlreadyExists => Resources.Localization.DirectoryAlreadyExists;
   public string NewDirectoryName =>  Resources.Localization.NewDirectoryName;
   public string DirectoryMustNotEndWithDot => Resources.Localization.DirectoryMustNotEndsWithDot;
   public string NameCannotBeEmpty => Resources.Localization.NameCannotBeEmpty;
   public string DirectoryUsingReservedStrings => Resources.Localization.DirectoryUsingReservedString;
   public string NameUsingInvalidCharacters => Resources.Localization.NameUsingsInvalidCharecters;
   public string NameTooLong =>  Resources.Localization.NameTooLong;
   public string CannotCreateDirectoryHere  => Resources.Localization.CannotCreateDirectoryHere;
   public string Create => Resources.Localization.Create;
   public string NewFile => Resources.Localization.NewFile;
   public string InsertFileName => Resources.Localization.InsertFileName;
   public string FileAlreadyExists => Resources.Localization.FileAlreadyExists;
   public string NewFileMenuItem => Resources.Localization.NewFileMenuItem;
   public string CreateFile => Resources.Localization.CreateFile;
   public string FileMustNotEndWithDot => Resources.Localization.FileMustNotEndWithDot;
   public string FileUsingReservedStrings => Resources.Localization.FileUsingReservedStrings;

   public void ChangeLanguage(CultureInfo culture)
   {
      CultureInfo.CurrentCulture = culture;
      CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
      manager.ChangeCurrentCulture(culture);
   }

}