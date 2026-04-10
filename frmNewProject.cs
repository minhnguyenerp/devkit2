using devkit2.Common;
using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmNewProject : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Project { get; set; } = null;

        public frmNewProject()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            this.ActiveControl = comboBoxTemplate;
        }

        private void frmNewProject_Load(object sender, EventArgs e)
        {
            JsonArray array = new JsonArray();
            comboBoxTemplate.Items.Add(new ValueName("Golang, VSCode", "Golang, VSCode")
            {
                Tag = new JsonObject
                {
                    ["ProjectName"] = "`ProjectName`",
                    ["Program"] = "VSCode",
                    ["Version"] = Sysconf.Instance.GetApplication("VSCode")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                    ["Profile"] = new JsonObject
                    {
                        ["WorkingDirectory"] = "`WorkingDirectory`",
                        ["StartupFile"] = "`WorkingDirectory`",
                    },
                    ["Environments"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["Program"] = "Git",
                            ["Version"] = Sysconf.Instance.GetApplication("Git")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                        new JsonObject
                        {
                            ["Program"] = "Go",
                            ["Version"] = Sysconf.Instance.GetApplication("Go")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                    },
                }
            });
            comboBoxTemplate.Items.Add(new ValueName("RustGcc, VSCode", "RustGcc, VSCode")
            {
                Tag = new JsonObject
                {
                    ["ProjectName"] = "`ProjectName`",
                    ["Program"] = "VSCode",
                    ["Version"] = Sysconf.Instance.GetApplication("VSCode")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                    ["Profile"] = new JsonObject
                    {
                        ["WorkingDirectory"] = "`WorkingDirectory`",
                        ["StartupFile"] = "`WorkingDirectory`",
                    },
                    ["Environments"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["Program"] = "Git",
                            ["Version"] = Sysconf.Instance.GetApplication("Git")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                        new JsonObject
                        {
                            ["Program"] = "RustGcc",
                            ["Version"] = Sysconf.Instance.GetApplication("RustGcc")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                        new JsonObject
                        {
                            ["Program"] = "Winlibs",
                            ["Version"] = Sysconf.Instance.GetApplication("Winlibs")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                    },
                }
            });
            comboBoxTemplate.Items.Add(new ValueName("RustMsvc, VSCode", "RustMsvc, VSCode")
            {
                Tag = new JsonObject
                {
                    ["ProjectName"] = "`ProjectName`",
                    ["Program"] = "VSCode",
                    ["Version"] = Sysconf.Instance.GetApplication("VSCode")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                    ["Profile"] = new JsonObject
                    {
                        ["WorkingDirectory"] = "`WorkingDirectory`",
                        ["StartupFile"] = "`WorkingDirectory`",
                    },
                    ["Environments"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["Program"] = "Git",
                            ["Version"] = Sysconf.Instance.GetApplication("Git")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                        new JsonObject
                        {
                            ["Program"] = "RustMsvc",
                            ["Version"] = Sysconf.Instance.GetApplication("RustMsvc")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                        new JsonObject
                        {
                            ["Program"] = "Msvc",
                            ["Version"] = Sysconf.Instance.GetApplication("Msvc")?.AvailableVersions?.FirstOrDefault()?.Value ?? "",
                            ["Profile"] = null,
                            ["Run"] = false,
                        },
                    },
                }
            });
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtProjectName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please input the project name!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (comboBoxTemplate.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a project template!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (textBoxDirectory.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please select a directory!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var map = new Dictionary<string, string>
            {
                ["`ProjectName`"] = txtProjectName.Text.Trim(),
                ["`WorkingDirectory`"] = textBoxDirectory.Text.Trim(),
            };

            var projectTemplate = (comboBoxTemplate.SelectedItem as ValueName)?.Tag as JsonObject;
            if (projectTemplate != null)
            {

                ReplaceAll(projectTemplate, map);
                Project = projectTemplate;
                Project["GUID"] = Guid.NewGuid().ToString();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ReplaceAll(JsonNode node, Dictionary<string, string> map)
        {
            if (node is JsonObject obj)
            {
                foreach (var kvp in obj.ToList())
                {
                    var key = kvp.Key;
                    var child = kvp.Value;

                    if (child is JsonValue val)
                    {
                        var str = val.ToString();
                        foreach (var kv in map)
                        {
                            str = str.Replace(kv.Key, kv.Value);
                        }
                        obj[key] = str;
                    }
                    else if (child != null)
                    {
                        ReplaceAll(child, map);
                    }
                }
            }
            else if (node is JsonArray arr)
            {
                foreach (var item in arr)
                {
                    if (item != null)
                        ReplaceAll(item, map);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxDirectory.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
