﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Stores the entries in the icon bar margin. Multiple icon bar margins
	/// can use the same manager if split view is used.
	/// </summary>
	public class IconBarManager : IBookmarkMargin
	{
		ObservableCollection<IBookmark> bookmarks = new ObservableCollection<IBookmark>();
		
		public IconBarManager()
		{
			bookmarks.CollectionChanged += bookmarks_CollectionChanged;
		}
		
		public IList<IBookmark> Bookmarks {
			get { return bookmarks; }
		}
		
		void bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Redraw();
		}
		
		public void Redraw()
		{
			if (RedrawRequested != null)
				RedrawRequested(this, EventArgs.Empty);
		}
		
		public event EventHandler RedrawRequested;
		
		public void UpdateClassMemberBookmarks(IParsedFile parseInfo)
		{
			for (int i = bookmarks.Count - 1; i >= 0; i--) {
				if (bookmarks[i] is EntityBookmark)
					bookmarks.RemoveAt(i);
			}
			if (parseInfo == null)
				return;
			foreach (ITypeDefinition c in parseInfo.TopLevelTypeDefinitions) {
				AddEntityBookmarks(c);
			}
		}
		
		void AddEntityBookmarks(ITypeDefinition c)
		{
			if (c.IsSynthetic) return;
			if (!c.Region.IsEmpty) {
				bookmarks.Add(new EntityBookmark(c));
			}
			foreach (ITypeDefinition innerClass in c.NestedTypes) {
				AddEntityBookmarks(innerClass);
			}
			foreach (IMember m in c.Members) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bookmarks.Add(new EntityBookmark(m));
			}
		}
	}
}
