using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryHeader : MonoBehaviour
{
	public Text text;
	public Image hiddenCategoryImage;

    public void Setup(JeopardyData.CategoryData category, bool isHidden)
	{
		text.text = category.categoryName.ToUpper();

		hiddenCategoryImage.gameObject.SetActive(isHidden);
	}

	public void ShowCategory()
	{
		hiddenCategoryImage.gameObject.SetActive(false);
	}
}
