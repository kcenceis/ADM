using System.ComponentModel;

namespace ADM
{
    class List_ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _table_id;
        public string table_id
        {
            get { return _table_id; }
            set
            {
                _table_id = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("table_id"));
                }
            }
        }
        private string _title;
        public string title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("title"));
                }
            }
        }
        private string _percent;
        public string percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("percent"));
                }
            }
        }
        private string _size;
        public string size
        {
            get { return _size; }
            set
            {
                _size = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("size"));
                }
            }
        }
        private string _status;
        public string status
        {
            get { return _status; }
            set
            {
                _status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("status"));
                }
            }
        }
        private string _type;
        public string type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("type"));
                }
            }
        }
        private string _time;
        public string time
        {
            get { return _time; }
            set
            {
                _time = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("time"));
                }
            }
        }
        private string _download_speed;
        public string download_speed
        {
            get { return _download_speed; }
            set
            {
                _download_speed = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("download_speed"));
                }
            }
        }
        private string _upload_speed;
        public string upload_speed
        {
            get { return _upload_speed; }
            set
            {
                _upload_speed = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("upload_speed"));
                }
            }
        }
        private string _peers;
        public string peers
        {
            get { return _peers; }
            set
            {
                _peers = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("peers"));
                }
            }
        }
    }
}
