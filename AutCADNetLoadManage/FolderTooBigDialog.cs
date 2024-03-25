using System.Text;
using System.Windows.Forms;

namespace AutoCADNetLoadManager
{
	public class FolderTooBigDialog
	{
		public static DialogResult Show(string folderPath, long sizeInMB)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Folder [" + folderPath + "]");
			stringBuilder.AppendLine("is " + sizeInMB + "MB large.");
			stringBuilder.AppendLine("AddinManager will attempt to copy all the files to temp folder");
			stringBuilder.AppendLine("Select [Yes] to copy all the files to temp folder");
			stringBuilder.AppendLine("Select [No] to only copy test script DLL");
			string text = stringBuilder.ToString();
			return MessageBox.Show(text, Resources.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
		}
	}
}
