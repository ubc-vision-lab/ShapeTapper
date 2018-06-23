using UnityEngine;
using System.Collections.Generic;


// [CreateAssetMenu(fileName = "Data", menuName = "Data/ColumnGroup", order = 1)]
public abstract class ColumnGroup : ScriptableObject
{
	public string objectName = "New ColumnGroup";
	public List<Column> columnGroup;
}