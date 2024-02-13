using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FallGuysStats {
	/// <summary>
	/// Use the Manager to managing multiple Toast widgets
	/// </summary>
	public static class ToastManager {
		public const byte MAX_TOASTS_ALLOWED = 6;
		internal static Toast CurrentToast;

		/// <summary>
		/// Get all toasts displaying
		/// </summary>
		public static ToastCollection ToastCollection { get; private set; } = new ToastCollection();

		/// <summary>
		/// Add toast to collection
		/// </summary>
		internal static void AddToCollection() {
			if (ToastCollection.Count >= MAX_TOASTS_ALLOWED) return;
			if (string.IsNullOrEmpty(CurrentToast.Caption)) {
				throw new ArgumentException("Text property is required to display Toast");
			}

			CurrentToast._eunmaToast.Toast = CurrentToast;
			CurrentToast._eunmaToast.Caption = CurrentToast.Caption;
			CurrentToast._eunmaToast.Description = CurrentToast.Description;
			CurrentToast._eunmaToast.ToastFont = CurrentToast.ToastFont;
			CurrentToast._eunmaToast.Thumbnails = CurrentToast.Thumbnail;
			CurrentToast._eunmaToast.AppOwnerIcon = CurrentToast.AppOwnerIcon;
			CurrentToast._eunmaToast.ToastDuration = CurrentToast.ToastDuration;
			CurrentToast._eunmaToast.ToastAnimation = CurrentToast.ToastAnimation;
			CurrentToast._eunmaToast.ToastCloseStyle = CurrentToast.ToastCloseStyle;
			CurrentToast._eunmaToast.ToastTheme = CurrentToast.ToastThemeStyle;
			CurrentToast._eunmaToast.ToastSound = CurrentToast.ToastSound;
			CurrentToast._eunmaToast.IsMuted = CurrentToast.IsMuted;

			CurrentToast.SequentialId = ToastCollection.NextSequentialId(CurrentToast.ToastPosition);
			SetLocation(CurrentToast.ToastPosition);

			ToastCollection.Add(CurrentToast);
			CurrentToast._eunmaToast.Show(Toast.Window);
		}

		private static void SetLocation(ToastPosition toastPosition) {
			switch (toastPosition) {
				case ToastPosition.TopRight: {
					var workingArea = Screen.GetWorkingArea(CurrentToast._eunmaToast);

					if (ToastCollection.Count == 0) {
						CurrentToast._eunmaToast.Left = workingArea.Right - CurrentToast._eunmaToast.Width - CurrentToast.GetHorizontalMargin();
						CurrentToast._eunmaToast.Top = workingArea.Top + CurrentToast.GetVerticalMargin();
					} else {
						var collection = ToastCollection.GetTopRightToasts();
						var enumerable = collection as List<Toast> ?? collection.ToList();
						if (enumerable.Count == 0) {
							CurrentToast._eunmaToast.Left = workingArea.Right - CurrentToast._eunmaToast.Width - CurrentToast.GetHorizontalMargin();
							CurrentToast._eunmaToast.Top = workingArea.Top + CurrentToast.GetVerticalMargin();
						} else {
							CurrentToast._eunmaToast.Left = workingArea.Right - CurrentToast._eunmaToast.Width - CurrentToast.GetHorizontalMargin();
							CurrentToast._eunmaToast.Top = workingArea.Top + CurrentToast.SequentialId * CurrentToast._eunmaToast.Height + CurrentToast.SequentialId * CurrentToast.GetVerticalMargin() + CurrentToast.GetVerticalMargin();
						}
					}
				}
					break;
				case ToastPosition.BottomRight: {
					var workingArea = Screen.GetWorkingArea(CurrentToast._eunmaToast);
					if (ToastCollection.Count == 0) {
						CurrentToast._eunmaToast.Location = new Point(workingArea.Right - CurrentToast._eunmaToast.Size.Width - CurrentToast.GetHorizontalMargin(),
							workingArea.Bottom - CurrentToast._eunmaToast.Size.Height - CurrentToast.GetVerticalMargin());
					} else {
						var collection = ToastCollection.GetBottomRightToasts();
						var enumerable = collection as List<Toast> ?? collection.ToList();
						if (enumerable.Count == 0) {
							CurrentToast._eunmaToast.Location = new Point(workingArea.Right - CurrentToast._eunmaToast.Size.Width - CurrentToast.GetHorizontalMargin(),
								workingArea.Bottom - CurrentToast._eunmaToast.Size.Height - CurrentToast.GetVerticalMargin());
						} else {
							CurrentToast._eunmaToast.Location = new Point(workingArea.Right - CurrentToast._eunmaToast.Size.Width - CurrentToast.GetHorizontalMargin(),
								workingArea.Bottom - CurrentToast._eunmaToast.Size.Height - CurrentToast._eunmaToast.Size.Height * CurrentToast.SequentialId - CurrentToast.GetVerticalMargin() * CurrentToast.SequentialId - CurrentToast.GetVerticalMargin());
						}	
					}
				}
					break;
			}
		}

	}

	public class ToastCollection : ICollection<Toast> {
		private readonly List<Toast> _privateList;

		/// <summary>
		/// Initialize empty Toast collection
		/// </summary>
		public ToastCollection() {
			_privateList = new List<Toast>(ToastManager.MAX_TOASTS_ALLOWED);
		}

		/// <summary>
		/// Initialize Toast collection from other collection
		/// </summary>
		/// <param name="other"></param>
		public ToastCollection(ToastCollection other) {
			if (other != null) {
                _privateList = new List<Toast>(other._privateList) {
                    Capacity = ToastManager.MAX_TOASTS_ALLOWED
                };
            }
		}

		/// <summary>
		/// Gets or sets value of specific Toast in collection by index
		/// </summary>
		/// <param name="index0"></param>
		/// <returns></returns>
		public Toast this[int index0] {
			get => _privateList[index0];
			set => _privateList[index0] = value;
		}

		/// <summary>
		/// Locate index of Toast in collection
		/// </summary>
		/// <param name="toast"></param>
		/// <returns></returns>
		public int Locate(Toast toast) {
			return _privateList.IndexOf(toast);
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Toast> GetEnumerator() {
			return _privateList.GetEnumerator();
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		// public List<Toast> Get() {
		// 	return _privateList;
		// }

		/// <inheritdoc />
		/// <summary>
		/// Add new Toast item to collection
		/// </summary>
		/// <param name="item">A Toast object</param>
		public void Add(Toast item) {
			_privateList.Add(item);
			ToastAdded?.Invoke(this, new ToastChangedEventArgs(item));
		}

		/// <inheritdoc />
		/// <summary>
		/// This will truncate collection, means all items will be cleaned
		/// </summary>
		public void Clear() {
			_privateList.Clear();
			if (_privateList.Count == 0) {
				CollectionTruncated?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool Contains(Toast item) {
			if(item == null) throw new NullReferenceException("Toast item cannot be null");
			if (_privateList.Count == 0) return false;
			foreach (var toast in _privateList) {
				if (toast.Guid.Contains(toast.Guid)) {
					return true;
				}
			}
			return false;
		}

		public void CopyTo(Toast[] array, int arrayIndex) {
			_privateList.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		/// <summary>
		/// Remove Toast item from current collection
		/// </summary>
		/// <param name="toast"></param>
		/// <returns></returns>
		public bool Remove(Toast toast) {
			var res = _privateList.Remove(toast);
			if (res) {
				ToastRemoved?.Invoke(this, new ToastChangedEventArgs(toast));
				if (_privateList.Count == 0) {
					CollectionTruncated?.Invoke(this, EventArgs.Empty);
				}
			}
			return res;
		}

		/// <summary>
		/// Remove all Toasts matched condition in predicate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public int RemoveAll(Predicate<Toast> match) {
			var num = _privateList.RemoveAll(match);
			if (_privateList.Count == 0) {
				CollectionTruncated?.Invoke(this, EventArgs.Empty);
			}
			return num;
		}
		
		public int NextSequentialId(ToastPosition toastPosition) {
            for (int i = 0; i < ToastManager.MAX_TOASTS_ALLOWED; i++) {
                if (!_privateList.Exists(toast => toast.ToastPosition == toastPosition && toast.SequentialId == i)) return i;
            }
            return 0;
        }

		/// <summary>
		/// Get all Top-Right Toasts in collection
		/// </summary>
		/// <returns>Top-Right Toast list</returns>
		public IEnumerable<Toast> GetTopRightToasts() {
			if (_privateList.Count == 0) yield break;
			foreach (var toast in _privateList) {
				if (toast.ToastPosition == ToastPosition.TopRight) {
					yield return toast;
				}
			}
		}

		/// <summary>
		/// Get all Bottom-Right Toasts in collection
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Toast> GetBottomRightToasts() {
			if(_privateList.Count == 0) yield break;
			foreach (var toast in _privateList) {
				if (toast.ToastPosition == ToastPosition.BottomRight) {
					yield return toast;
				}
			}
		}

		public int Count => _privateList.Count;
		public bool IsReadOnly => false;

		public delegate void ToastAddedEventHandler(object sender, ToastChangedEventArgs e);

		public event ToastAddedEventHandler ToastAdded;

		public delegate void ToastRemovedEventHandler(object sender, ToastChangedEventArgs e);

		public event ToastRemovedEventHandler ToastRemoved;

		public delegate void CollectionTruncatedEventHandler(object sender, EventArgs e);

		public event CollectionTruncatedEventHandler CollectionTruncated;
	}

	public class ToastChangedEventArgs : EventArgs {
		private readonly Toast _toast;

		public Toast Toast => _toast;

		public ToastChangedEventArgs(Toast toast) {
			_toast = toast;
		}
	}
}
