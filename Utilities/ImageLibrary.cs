using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WashingMachine
{
    public sealed class ImageLibrary
    {
        #region Constants
        private const string ImagesFileName = "UnitImages.json";
        #endregion

        #region Properties
        private static readonly Lazy<ImageLibrary> singletonInstance = new Lazy<ImageLibrary>(() => new ImageLibrary());
        public static ImageLibrary Instance => singletonInstance.Value;
        private ConcurrentDictionary<BaseUnit.MachineUnitImageType, string> unitImageFileNames = default;
        private ConcurrentDictionary<BaseUnit.MachineUnitImageType, Image> unitImages = new ConcurrentDictionary<BaseUnit.MachineUnitImageType, Image>();

        public Image this[BaseUnit.MachineUnitImageType unitType]
        {
            get
            {
                if (!unitImages.ContainsKey(unitType))
                {
                    LoadImage(unitType);
                }

                return unitImages[unitType];
            }
        }
        #endregion

        #region Construction
        private ImageLibrary() { }
        public bool Initialize()
        {
            return LoadFromFile();
        }
        #endregion

        #region Methods
        private bool LoadFromFile()
        {
            try
            {
                string jsonFilePath = Path.Combine(Application.StartupPath, "Configuration", ImagesFileName);
                using (var reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    unitImageFileNames = JsonConvert.DeserializeObject<ConcurrentDictionary<BaseUnit.MachineUnitImageType, string>>(jsonString);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
                return false;
            }

            Logger.Instance.LogInformation("Image library loaded.");
            return true;
        }

        private bool LoadImage(BaseUnit.MachineUnitImageType unitType)
        {
            try
            {
                var image = Image.FromFile(Path.Combine(Application.StartupPath, "Images", unitImageFileNames[unitType]));
                unitImages[unitType] = image;

                return image != null;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
                return false;
            }
        }
        #endregion
    }
}
