# OpcDataExplorer
An data explorer written in C#, monitor all tags by default, support filter, checkbox, and write value. Screenshot included.

# Introduction
Most of the Opc data explorer developed in the 90’s last century, and never upgraded, such as Iconics Opc dataSpy, MatrikonOPC Explorer, and the most early "OPC Client" developed by FactorySoft.

They are developed in the age CPU is slow, memory is small, you must save them.

They are great software, and are still widely used in the industry monitor and control fields.

However, you need some more convenient tools today. 

That's why I wrote this piece of thing. Hope it will be helpful to others.

# About OpcDataExplorer

It's written in C#, and based on [ObjectListView](https://github.com/jessejohnston/ObjectListView) by [jessejohnston](https://github.com/jessejohnston).

There're some screenshot included,all said one picture says more than thousand words.

## Tree structure


By default, it assumes the tag name is divided by '.', such as:

```
L01_L0101.HV.node1
L01_L0101.HV.node1.dipoint1
L01_L0101.HV.node1.aipoint2

```

```cs
		private string _delimiter = ".";
		public string Delimiter 
		{ 
            get { return _delimiter; }
			set { _delimiter = value; }
		}
```

### Guess delimiter

It will guess the delimiter when starting up, and if you tag's name is delimited by someting, such as '.' or '|', etc.

```cs
		private void GetDelimiter()
		{
			string root = "";
			string name;

            oPCBrowser.ShowBranches();
			if (oPCBrowser.Count >= 1)
			{
				root = oPCBrowser.Item(1);
				Console.WriteLine("root is: " + root);

				if (root != "")
				{
					oPCBrowser.ShowLeafs(true);
					for (int i = 1; i <= oPCBrowser.Count; i++)
					{
						name = oPCBrowser.Item(i);
						Console.WriteLine("name is: " + name);
						if (name != "")
						{
							if (root != name)
							{
								int pos = name.IndexOf(root);
								string delimier = name.Substring(root.Length, 1);
								Console.WriteLine("delimier is: " + delimier);
                                if (!Char.IsLetterOrDigit(System.Convert.ToChar(delimier)))
								{
									Delimiter = delimier;
									return;
								}
							} else {

							}
						} else {
							
						}
					}

				} else {
					
				}
			}
		}
```
However, it does not alway work. If you are unhappy with this, make your own modification.

# ScreenShot
