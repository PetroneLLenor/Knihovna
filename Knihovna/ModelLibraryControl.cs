using Eto.Drawing;
using Eto.Forms;
using Rhino;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Knihovna
{
    public class ModelLibraryControl : Panel
    {
        private string _directoryPath = @"D:\Modely";
        private TreeView _treeView;
        private TreeItem _rootItem;
        private Button _refreshButton;
        private Button _settingsButton;
        private ImageView _imageView;
        private List<CheckBox> _typeCheckBoxes = new List<CheckBox>();

        public ModelLibraryControl()
        {
            var layout = new DynamicLayout();

            layout.BeginVertical(); // Vertical layout

            // Panel for buttons
            var buttonLayout = new DynamicLayout { Padding = new Padding(0, 0, 0, 10) };
            buttonLayout.BeginHorizontal();

            _refreshButton = new Button { Size = new Eto.Drawing.Size(20, 20) };
            string refreshImagePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "refresh-button.png");
            _refreshButton.Image = new Eto.Drawing.Bitmap(refreshImagePath);
            _refreshButton.Click += OnRefreshClick;
            buttonLayout.Add(_refreshButton, xscale: false);

            _settingsButton = new Button { Size = new Eto.Drawing.Size(20, 20) };
            string settingsImagePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "gear.png");
            _settingsButton.Image = new Eto.Drawing.Bitmap(settingsImagePath);
            _settingsButton.Click += OnSettingsClick;
            buttonLayout.Add(_settingsButton, xscale: false);

            buttonLayout.Add(null);
            buttonLayout.EndHorizontal();
            layout.Add(buttonLayout, yscale: false);

            // File Type Checkboxes
            var checkBoxLayout = new DynamicLayout { Padding = new Padding(0, 0, 0, 10) };
            checkBoxLayout.BeginHorizontal();
            var fileTypes = new[] { "3dm", "dwg", "obj", "fbx" };
            foreach (var fileType in fileTypes)
            {
                var checkBox = new CheckBox { Text = fileType, Checked = true };
                _typeCheckBoxes.Add(checkBox);
                checkBox.CheckedChanged += (sender, e) => OnRefreshClick(sender, e);
                checkBoxLayout.Add(checkBox, xscale: false);
            }
            checkBoxLayout.Add(null);
            checkBoxLayout.EndHorizontal();
            layout.Add(checkBoxLayout, yscale: false);

            // TreeView
            _treeView = new TreeView();
            _rootItem = new TreeItem();
            _rootItem.Text = _directoryPath;
            LoadModels(_directoryPath, _rootItem);
            _treeView.DataStore = _rootItem;
            _treeView.SelectionChanged += OnTreeSelectionChanged;
            _treeView.MouseDoubleClick += OnTreeDoubleClick;

            // ImageView
            _imageView = new ImageView();

            // Create a Splitter and add TreeView and ImageView to it
            var splitter = new Splitter();
            splitter.Orientation = Orientation.Vertical; // Add this line
            splitter.Panel1 = _treeView;
            splitter.Panel2 = _imageView;
            splitter.Position = 440; // Adjust this to set the initial position of the splitter
            layout.Add(splitter); // Add the Splitter to the layout



            Content = layout;
        }

        private void OnTreeSelectionChanged(object sender, EventArgs e)
        {
            var treeView = sender as TreeView;
            var selectedNode = treeView.SelectedItem as TreeItem;
            if (selectedNode != null && selectedNode.Tag != null)
            {
                string filePath = selectedNode.Tag.ToString();
                System.Drawing.Bitmap previewImage = null;

                if (filePath.EndsWith(".3dm"))
                {
                    previewImage = Rhino.FileIO.File3dm.ReadPreviewImage(filePath);
                }
                else if (filePath.EndsWith(".dwg"))
                    if (filePath.EndsWith(".obj"))
                        if (filePath.EndsWith(".fbx"))
                        {
                   
                    
                    // Handle dwg preview image if possible
                }

                if (previewImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        previewImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var etoBitmap = new Eto.Drawing.Bitmap(memoryStream);
                        _imageView.Image = etoBitmap;
                    }
                }
                else
                {
                    _imageView.Image = null;
                }
            }
            else
            {
                _imageView.Image = null;
            }
        }

        private void OnTreeDoubleClick(object sender, MouseEventArgs e)
        {
            var treeView = sender as TreeView;
            var selectedNode = treeView.SelectedItem as TreeItem;
            if (selectedNode != null && selectedNode.Tag != null)
            {
                string filePath = selectedNode.Tag.ToString();
                if (filePath.EndsWith(".3dm") || filePath.EndsWith(".dwg"))
                {
                    string command = $"_-Import \"{filePath}\" _Enter";
                    RhinoApp.RunScript(command, false);
                }
            }
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            _rootItem = new TreeItem();
            _rootItem.Text = _directoryPath;
            LoadModels(_directoryPath, _rootItem);
            _treeView.DataStore = _rootItem;
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            var dialog = new SelectFolderDialog();
            dialog.Directory = _directoryPath;
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                _directoryPath = dialog.Directory;
                OnRefreshClick(sender, e);
            }
        }

        private void LoadModels(string directoryPath, TreeItem treeItem)
        {
            try
            {
                var directories = Directory.GetDirectories(directoryPath);
                foreach (var directory in directories)
                {
                    var subTreeItem = new TreeItem();
                    subTreeItem.Text = Path.GetFileName(directory);
                    LoadModels(directory, subTreeItem);
                    if (subTreeItem.Children.Count > 0)
                    {
                        treeItem.Children.Add(subTreeItem);
                    }
                }

                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file).TrimStart('.').ToLower();
                    if (_typeCheckBoxes.Any(c => c.Text == extension && c.Checked.Value))
                    {
                        var fileTreeItem = new TreeItem();
                        fileTreeItem.Text = Path.GetFileName(file);
                        fileTreeItem.Tag = file;
                        treeItem.Children.Add(fileTreeItem);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle directory access exception
            }
        }
    }
}
