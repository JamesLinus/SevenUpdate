using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WindowsUI
{

  #region PUBLIC enums

    public enum TaskDialogIcons
    {
        None = ((int)VistaTaskDialogIcon.None),
        Information = ((int)VistaTaskDialogIcon.Information),
        Warning = ((int)VistaTaskDialogIcon.Warning),
        Error = ((int)VistaTaskDialogIcon.Error)
    };
    public enum TaskDialogButtons
    {
        YesNo = VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No,
        YesNoCancel = VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No | VistaTaskDialogCommonButtons.Cancel,
        OKCancel = VistaTaskDialogCommonButtons.Ok | VistaTaskDialogCommonButtons.Cancel,
        OK = VistaTaskDialogCommonButtons.Ok,
        Close = VistaTaskDialogCommonButtons.Close,
        Cancel = VistaTaskDialogCommonButtons.Cancel,
        None = VistaTaskDialogCommonButtons.None
    }
  #endregion

  public static class TaskDialog
  {
    // PUBLIC static values...
    static public bool VerificationChecked = false;
    static public int RadioButtonResult = -1;
    static public int CommandButtonResult = -1;
    static public int EmulatedFormWidth = 450;
    static public bool ForceEmulationMode = false;
    static public bool UseToolWindowOnXP = true;
    static public bool PlaySystemSounds = true;

    #region ShowTaskDialogBox
  
    static public DialogResult Show(string Title,
                                                 string MainInstruction,
                                                 string Content,
                                                 string ExpandedInfo,
                                                 string Footer,
                                                 string VerificationText,
                                                 string RadioButtons,
                                                 string CommandButtons,
                                                 TaskDialogButtons Buttons,
                                                 TaskDialogIcons MainIcon,
                                                 TaskDialogIcons FooterIcon)
    {
      if (VistaTaskDialog.IsAvailableOnThisOS && !ForceEmulationMode)
      {
        // [OPTION 1] Show Vista TaskDialog
        VistaTaskDialog vtd = new VistaTaskDialog();

        vtd.WindowTitle = Title;
        vtd.MainInstruction = MainInstruction;
        vtd.Content = Content;
        vtd.ExpandedInformation = ExpandedInfo;
        vtd.Footer = Footer;

        // Radio Buttons
        if (RadioButtons != "")
        {
          List<VistaTaskDialogButton> lst = new List<VistaTaskDialogButton>();
          string[] arr = RadioButtons.Split(new char[] { '|' });
          for (int i = 0; i < arr.Length; i++)
          {
            try
            {
              VistaTaskDialogButton button = new VistaTaskDialogButton();
              button.ButtonId = 1000 + i;
              button.ButtonText = arr[i];
              lst.Add(button);
            }
            catch (FormatException)
            {
            }
          }
          vtd.RadioButtons = lst.ToArray();
        }

        // Custom Buttons
        if (CommandButtons != "")
        {
          List<VistaTaskDialogButton> lst = new List<VistaTaskDialogButton>();
          string[] arr = CommandButtons.Split(new char[] { '|' });
          for (int i = 0; i < arr.Length; i++)
          {
            try
            {
              VistaTaskDialogButton button = new VistaTaskDialogButton();
              button.ButtonId = 2000 + i;
              button.ButtonText = arr[i];
              lst.Add(button);
            }
            catch (FormatException)
            {
            }
          }
          vtd.Buttons = lst.ToArray();
        }
        vtd.CommonButtons = ((VistaTaskDialogCommonButtons)Buttons);
        vtd.MainIcon = ((VistaTaskDialogIcon)MainIcon);
        vtd.FooterIcon = ((VistaTaskDialogIcon)FooterIcon);

        vtd.EnableHyperlinks = false;
        vtd.ShowProgressBar = false;
        vtd.AllowDialogCancellation = (Buttons == TaskDialogButtons.Cancel ||
                                       Buttons == TaskDialogButtons.Close ||
                                       Buttons == TaskDialogButtons.OKCancel ||
                                       Buttons == TaskDialogButtons.YesNoCancel);
        vtd.CallbackTimer = false;
        vtd.ExpandedByDefault = false;
        vtd.ExpandFooterArea = false;
        vtd.PositionRelativeToWindow = true;
        vtd.RightToLeftLayout = false;
        vtd.NoDefaultRadioButton = false;
        vtd.CanBeMinimized = false;
        vtd.ShowMarqueeProgressBar = false;
        vtd.UseCommandLinks = (CommandButtons != "");
        vtd.UseCommandLinksNoIcon = false;
        vtd.VerificationText = VerificationText;
        vtd.VerificationFlagChecked = false;
        vtd.ExpandedControlText = "Hide details";
        vtd.CollapsedControlText = "Show details";
        vtd.Callback = null;

        // Show the Dialog
        DialogResult result = (DialogResult)vtd.Show((vtd.CanBeMinimized ? null : Form.ActiveForm), out VerificationChecked, out RadioButtonResult);

        // if a command button was clicked, then change return result
        // to "DialogResult.OK" and set the CommandButtonResult
        if ((int)result >= 2000)
        {
          CommandButtonResult = ((int)result - 2000);
          result = DialogResult.OK;
        }
        if (RadioButtonResult >= 1000)
          RadioButtonResult -= 1000;  // deduct the ButtonID start value for radio buttons

        return result;
      }
      else
      {
        // [OPTION 2] Show Emulated Form
        frmTaskDialog td = new frmTaskDialog();
        td.Title = Title;
        td.MainInstruction = MainInstruction;
        td.Content = Content;
        td.ExpandedInfo = ExpandedInfo;
        td.Footer = Footer;
        td.RadioButtons = RadioButtons;
        td.CommandButtons = CommandButtons;
        td.Buttons = Buttons;
        td.MainIcon = MainIcon;
        td.FooterIcon = FooterIcon;
        td.VerificationText = VerificationText;
        td.Width = EmulatedFormWidth;
        td.BuildForm();
        DialogResult result = td.ShowDialog();

        RadioButtonResult = td.RadioButtonIndex;
        CommandButtonResult = td.CommandButtonClickedIndex;
        VerificationChecked = td.VerificationCheckBoxChecked;
        return result;
      }
    }
    #endregion

  
    #region MessageBox
  
    static public DialogResult Show(string Title,
                                          string MainInstruction,
                                          string Content,
                                          string ExpandedInfo,
                                          string Footer,
                                          string VerificationText,
                                          TaskDialogButtons Buttons,
                                          TaskDialogIcons MainIcon,
                                          TaskDialogIcons FooterIcon)
    {
      return Show(Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                               "", "", Buttons, MainIcon, FooterIcon);
    }
    
  
    static public DialogResult Show(string Title,
                                          string MainInstruction,
                                          string Content,
                                          TaskDialogButtons Buttons,
                                          TaskDialogIcons MainIcon)
    {
      return Show(Title, MainInstruction, Content, "", "", "", Buttons, MainIcon, TaskDialogIcons.Information);
    }

  
    #endregion

  
    #region ShowRadioBox
  
    static public int Show(string Title,
                                   string MainInstruction,
                                   string Content,
                                   string ExpandedInfo,
                                   string Footer,
                                   string VerificationText,
                                   string RadioButtons,
                                   TaskDialogIcons MainIcon,
                                   TaskDialogIcons FooterIcon)
    {
      DialogResult res = Show(Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                           RadioButtons, "", TaskDialogButtons.OKCancel,
                                           MainIcon, FooterIcon);
      if (res == DialogResult.OK)
        return RadioButtonResult;
      else
        return -1;
    }

  
    // Simple overloaded version
    static public int Show(string Title,
                                   string MainInstruction,
                                   string Content,
                                   string RadioButtons)
    {
      return Show(Title, MainInstruction, Content, "", "", "", RadioButtons, TaskDialogIcons.Information, TaskDialogIcons.Information);
    }
    #endregion

  
    #region ShowCommandBox
  
    static public int Show(string Title,
                                     string MainInstruction,
                                     string Content,
                                     string ExpandedInfo,
                                     string Footer,
                                     string VerificationText,
                                     string CommandButtons,
                                     bool ShowCancelButton,
                                     TaskDialogIcons MainIcon,
                                     TaskDialogIcons FooterIcon)
    {
      DialogResult res = Show(Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                           "", CommandButtons, (ShowCancelButton ? TaskDialogButtons.Cancel : TaskDialogButtons.None),
                                           MainIcon, FooterIcon);
      if (res == DialogResult.OK)
        return CommandButtonResult;
      else
        return -1;
    }

  
    // Simple overloaded version
    static public int Show(string Title, string MainInstruction, string Content, string CommandButtons, bool ShowCancelButton)
    {
      return Show(Title, MainInstruction, Content, "", "", "", CommandButtons, ShowCancelButton,
                            TaskDialogIcons.Information, TaskDialogIcons.Information);
    }

    #endregion

  
  }
}
