using System.Text;
using System.Collections;

namespace Maple.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DataParameterCollection : System.Collections.CollectionBase
    {

        private bool _hasReturnValue = false;
        /// <summary>
        /// 标识参数中是否存在返回值
        /// </summary>
        public bool HasReturnValue
        {
            get { return _hasReturnValue; }
        }

        public DataParameterCollection() { }

        public DataParameterCollection(params DataParameter[] dps)
        {
            Add(dps);
        }

        public void Add(DataParameter dp)
        {
            if (dp.Direction != System.Data.ParameterDirection.Input)
                this._hasReturnValue = true;
            List.Add(dp);
        }

        public void Add(params DataParameter[] dps)
        {
            foreach (DataParameter dp in dps)
            {
                Add(dp);
            }
        }

        public void Add(DataParameterCollection dpc)
        {
            foreach (DataParameter dp in dpc)
            {
                Add(dp);
            }
        }

        public DataParameter this[int index]
        {
            get { return (DataParameter)List[index]; }
            set { List[index] = value; }
        }

        public override bool Equals(object obj)
        {
            var dpc = (DataParameterCollection)obj;
            for (int i = 0; i < List.Count; i++)
            {
                if (!this[i].Equals(dpc[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (DataParameter dp in List)
            {
                sb.Append(dp.ToString());
                sb.Append(",");
            }
            if (List.Count > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}
