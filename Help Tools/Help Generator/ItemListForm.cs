﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Help_Generator
{
    partial class ItemListForm : Form
    {
        private Preprocessor _preprocessor;
        private List<string> _listTitles;
        private List<object> _listObjects;

        public ItemListForm(Preprocessor preprocessor,bool viewTopics)
        {
            InitializeComponent();

            _preprocessor = preprocessor;
            _listTitles = new List<string>();
            _listObjects = new List<object>();

            if( !viewTopics )
                radioButtonImages.Checked = true;
        }

        private void TopicListForm_Load(object sender,EventArgs e)
        {
            UpdateList(true);
        }

        private void radioButton_CheckedChanged(object sender,EventArgs e)
        {
            if( ((RadioButton)sender).Checked ) // only respond to the event for the now-checked radio button
            {
                EnableButtons(false);
                UpdateList(true);
            }
        }

        private void textBoxFilter_TextChanged(object sender,EventArgs e)
        {
            EnableButtons(false);
            UpdateList(false);
        }

        private void EnableButtons(bool itemSelected)
        {
            if( buttonView.Enabled != itemSelected )
            {
                buttonView.Enabled = itemSelected;
                buttonCopyFilename.Enabled = itemSelected;
                buttonDelete.Enabled = itemSelected;
            }
        }

        public void UpdateList(bool regenerateList)
        {
            if( regenerateList )
            {
                _listTitles.Clear();
                _listObjects.Clear();

                if( radioButtonTopics.Checked )
                {
                    foreach( Preprocessor.TopicPreprocessor topic in _preprocessor.GetAllTopics() )
                    {
                        _listTitles.Add(topic.Title);
                        _listObjects.Add(topic);
                    }
                }

                else
                {
                    foreach( Preprocessor.ImagePreprocessor image in _preprocessor.GetAllImages() )
                    {
                        _listTitles.Add(Path.GetFileName(image.Filename));
                        _listObjects.Add(image);
                    }
                }
            }


            bool useFilter = !String.IsNullOrWhiteSpace(textBoxFilter.Text);

            listViewItems.Clear();

            for( int i = 0; i < _listTitles.Count; i++ )
            {
                if( useFilter && _listTitles[i].IndexOf(textBoxFilter.Text,StringComparison.InvariantCultureIgnoreCase) < 0 )
                    continue;

                ListViewItem lvi = listViewItems.Items.Add(_listTitles[i]);
                lvi.Tag = _listObjects[i];
            }
        }

        private void listViewItems_ItemSelectionChanged(object sender,ListViewItemSelectionChangedEventArgs e)
        {
            EnableButtons(e.IsSelected);
        }

        private void buttonView_Click(object sender,EventArgs e)
        {
            if( listViewItems.SelectedItems.Count == 0 )
                return;

            object selectedTag = listViewItems.SelectedItems[0].Tag;

            if( radioButtonTopics.Checked )
                ((MainForm)this.ParentForm).ShowOrCreateForm(selectedTag);

            else
            {
                string imageFilename = ((Preprocessor.ImagePreprocessor)selectedTag).Filename;
                Process.Start(imageFilename);
            }
        }

        private void buttonCopyFilename_Click(object sender,EventArgs e)
        {
            object selectedTag = listViewItems.SelectedItems[0].Tag;
            string filename = radioButtonTopics.Checked ? ((Preprocessor.TopicPreprocessor)selectedTag).Filename : ((Preprocessor.ImagePreprocessor)selectedTag).Filename;

            Clipboard.Clear();
            Clipboard.SetText(Path.GetFileName(filename));
        }

        private void buttonDelete_Click(object sender,EventArgs e)
        {
            object selectedTag = listViewItems.SelectedItems[0].Tag;

            if( radioButtonTopics.Checked )
            {
                foreach( Form form in this.ParentForm.MdiChildren )
                {
                    if( form is TopicEditForm && ((TopicEditForm)form).PreprocessedTopic == selectedTag )
                    {
                        MessageBox.Show("You cannot delete the topic while it is being edited.");
                        return;
                    }
                }
            }

            string filename = radioButtonTopics.Checked ? ((Preprocessor.TopicPreprocessor)selectedTag).Filename : ((Preprocessor.ImagePreprocessor)selectedTag).Filename;

            if( MessageBox.Show(String.Format("Are you sure that you want to delete {0}?",filename),this.Text,MessageBoxButtons.YesNo) == DialogResult.Yes )
            {
                try
                {
                    File.Delete(filename);
                    _preprocessor.Refresh();
                    ((MainForm)this.ParentForm).UpdateItemLists();
                }

                catch( Exception exception )
                {
                    MessageBox.Show(String.Format("There was an error deleting {0}\n\n{1}",filename,exception.Message));
                }
            }
        }
    }
}
