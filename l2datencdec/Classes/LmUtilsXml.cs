#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Collections.ObjectModel;

#endregion

namespace LmUtils
{
	public class Xml
	{
		#region Static

		#region CreateNew / AddElement
		public static XmlElement CreateNew (ref XmlDocument xml)
		{
			xml = new XmlDocument ();
			xml.LoadXml ("<root></root>");
			return xml.DocumentElement;
		}

		public static XmlElement AddElement (string name, XmlElement parent)
		{
			XmlDocument xml = parent.OwnerDocument;
			XmlElement node = xml.CreateElement (name);
			parent.AppendChild (node);
			return node;
		} 
		#endregion

		#region GetNode, nodes / CreateNode
		public static XmlElement GetNode (XmlElement root, string node_path)
		{
			Collection<StringUtilities.PathClassIdentity> path = StringUtilities.ParseClassPath (node_path);

			XmlElement cur_root = root;
			foreach (StringUtilities.PathClassIdentity identity in path)
			{
				try
				{
					cur_root = cur_root.GetElementsByTagName (identity.name)[identity.index] as XmlElement;
				}
				catch
				{
					return null;
				}
			}

			return cur_root;
		}

		public static XmlNodeList GetNodes (XmlElement root, string node_path)
		{
			Collection<StringUtilities.PathClassIdentity> path = StringUtilities.ParseClassPath (node_path);

			XmlElement cur_root = root;
			StringUtilities.PathClassIdentity last_identity = null;
			foreach (StringUtilities.PathClassIdentity identity in path)
			{
				if (identity.index == -1 || identity == path[path.Count - 1])
				{
					last_identity = identity;
					break;
				}

				try
				{
					cur_root = cur_root.GetElementsByTagName (identity.name)[identity.index] as XmlElement;
					if (cur_root == null)
						return null;
				}
				catch
				{
					return null;
				}
			}

			return cur_root.GetElementsByTagName (last_identity.name);
		}

		public static XmlElement CreateNode (XmlElement root, string node_path)
		{
			Collection<StringUtilities.PathClassIdentity> path = StringUtilities.ParseClassPath (node_path);

			XmlElement cur_root = root;
			foreach (StringUtilities.PathClassIdentity identity in path)
			{
				if (identity.index < 0)
				{
					cur_root = Xml.AddElement (identity.name, cur_root);
					break;
				}
				else
				{
					XmlNodeList nodes = cur_root.GetElementsByTagName (identity.name);
					if (nodes.Count <= identity.index)
					{
						for (int k = nodes.Count; k <= identity.index; k++)
						{
							Xml.AddElement (identity.name, cur_root);
						}
					}

					cur_root = cur_root.GetElementsByTagName (identity.name)[identity.index] as XmlElement;
				}
			}

			return cur_root;
		}
		#endregion

		#endregion

		#region Constructors, destructor...
		public Xml () : this (null)
		{
		}

		public Xml (string file_name)
		{
			this.FileName = file_name;
			this.Reload ();
		}
		#endregion

		#region Save (virtual) / load (virtual)
		public virtual void Reload ()
		{
			XmlDocument new_xml = null;
			try
			{
				new_xml = new XmlDocument ();
				new_xml.Load (this.FileName);
			}
			catch
			{
				Xml.CreateNew (ref new_xml);
			}
			finally
			{
				this.Document = new_xml;
			}
		}

		public virtual void Save ()
		{
			this.Document.Save (this.FileName);
		} 
		#endregion

		#region GetNode, nodes / CreateNode
		public XmlElement GetNode (string path)
		{
			return Xml.GetNode (this.Root, path);
		}

		public XmlNodeList GetNodes (string path)
		{
			return Xml.GetNodes (this.Root, path);
		}

		public XmlElement CreateNode (string path)
		{
			return Xml.CreateNode (this.Root, path);
		}
		#endregion

		#region Accessors

		#region this (virtual)
		public virtual string this[string name]
		{
			get
			{
				XmlElement node = this.GetNode (name);
				if (node == null)
				{
					if (this.ThisCanThrowExeptions)
						throw new NullReferenceException ();

					return null;
				}

				return node.InnerText;
			}
			set
			{
				XmlElement node = this.GetNode (name);
				if (node == null)
					node = this.CreateNode (name);

				node.InnerText = value;
			}
		} 
		#endregion

		#region FileName (virtual)
		protected string file_name;

		/// <summary>
		/// Get / set file name
		/// </summary>
		/// <value></value>
		public virtual string FileName
		{
			get
			{
				return this.file_name;
			}
			set
			{
				this.file_name = value;
			}
		}
		#endregion

		#region Root (virtual)
		protected XmlElement root;

		/// <summary>
		/// Get/set
		/// </summary>
		/// <value></value>
		public virtual XmlElement Root
		{
			get
			{
				return this.root;
			}
			set
			{
				this.root = value;
			}
		}
		#endregion

		#region Xml (virtual)
		protected XmlDocument xml;

		/// <summary>
		/// Get/set
		/// </summary>
		/// <value></value>
		public XmlDocument Document
		{
			get
			{
				return this.xml;
			}
			set
			{
				this.xml = value;
				this.Root = this.xml.DocumentElement;
			}
		}
#endregion

		#region ThisCanThrowExeptions
		private bool this_can_throw_exceptions = false;

		/// <summary>
		/// Get/set
		/// </summary>
		/// <value></value>
		public bool ThisCanThrowExeptions
		{
			get
			{
				return this.this_can_throw_exceptions;
			}
			set
			{
				this.this_can_throw_exceptions = value;
			}
		}
		#endregion

		#endregion
	}
}
