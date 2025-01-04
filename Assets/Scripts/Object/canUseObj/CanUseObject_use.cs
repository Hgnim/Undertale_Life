using static CoreData.CData.Player;
using static CoreData.CData;
using static CoreData.ToolClass;

public static class CanUseObj_use
{

	public static void UseObject(int ItemID) {
		switch (ObjTagFind(BagItem.GetItemTag(ItemID), Objects.CPObjTagHead.useMode)) {
			case "hand":
				Player.MainItem.PickedItem(ItemID, MainItem.PickedLoc.hand);
				break;
		}
	}
	public static void DropObject(int ItemID) {
		Player.BagItem.RemoveItem(ItemID);
	}
}
