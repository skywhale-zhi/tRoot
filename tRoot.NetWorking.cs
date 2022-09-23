using tRoot.Content.Items.Consumables.Potions;
using tRoot.Content.NPCs.Person_Pets;
using System.IO;
using Terraria;

namespace tRoot
{
    partial class tRoot
	{
		internal enum MessageType : byte
		{
		}


		//重写此方法以处理为此mod发送的网络数据包。
		//TODO：将OOP包引入tML，以避免这种类级硬代码。
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
		}
	}
}
