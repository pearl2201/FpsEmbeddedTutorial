using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace Assets.Scripts.Share
{
	public struct LoginRequestData : IDarkRiftSerializable
	{
		public string Name;

		public LoginRequestData(string name)
		{
			Name = name;
		}

		public void Deserialize(DeserializeEvent e)
		{
			Name = e.Reader.ReadString();
		}

		public void Serialize(SerializeEvent e)
		{
			e.Writer.Write(Name);
		}
	}
}
