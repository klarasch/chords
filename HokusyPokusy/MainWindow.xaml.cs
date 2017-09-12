using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Controls = System.Windows.Controls;
using System.Windows.Input;
using Imaging = System.Windows.Media.Imaging;
using Shapes = System.Windows.Shapes;

public partial class MainWindow : Window
{
	/// <summary>
	/// Nota z pohledu GUI.
	/// </summary>
	class _Note
	{
		public Controls.Image Head;
		public Controls.Image Accidental;
	}

	App _app;

	private Imaging.BitmapImage _head = new Imaging.BitmapImage(new Uri("Resources/note.png", UriKind.Relative));
	private Imaging.BitmapImage _flat = new Imaging.BitmapImage(new Uri("Resources/flat_centered.png", UriKind.Relative));
	private Imaging.BitmapImage _sharp = new Imaging.BitmapImage(new Uri("Resources/sharp_centered.png", UriKind.Relative));
	private Imaging.BitmapImage _doubleSharp = new Imaging.BitmapImage(new Uri("Resources/doublesharp_centered.png", UriKind.Relative));
	private Imaging.BitmapImage _doubleFlat = new Imaging.BitmapImage(new Uri("Resources/doubleflat_centered.png", UriKind.Relative));

	/// <summary>
	/// Množina viditelných objektů (not a jejich posuvek).
	/// </summary>
	private HashSet<Controls.Image> _visibleImages = new HashSet<Controls.Image>();

	/// <summary>
	/// List zobrazených not.
	/// </summary>
	private List<_Note> _notes = new List<_Note>();

	/// <summary>
	/// Asociace klikatelných obdélníků s jejich notami.
	/// </summary>
	private Dictionary<Shapes.Rectangle, _Note> _rectangles = new Dictionary<Shapes.Rectangle, _Note>();

	/// <summary>
	/// Obsluha kliknutí na mezeru či linku (zobrazování not a jeich posuvek).
	/// </summary>
	private void _SpaceClick(object sender, MouseButtonEventArgs e) 
	{
		var note = _rectangles[sender as Shapes.Rectangle];
		var head = note.Head;
		head.Visibility = Visibility.Visible;
		head.Source = _head;  // po prvním kliknutí na mezeru/linku se zobrazí samotná nota
		_visibleImages.Add(head);  // a přidá se se do tabulky viditelných objektů
		var acc = note.Accidental;
		// další kliky točí zobrazení křížku, béčka, dvojkřížku a dvojbéčka
		if (acc.Source == null && acc.Visibility == Visibility.Collapsed) {
			acc.Visibility = Visibility.Visible;
			_visibleImages.Add(acc);
		}
		else if (acc.Source == null) {
			acc.Source = _sharp;
		}
		else if (acc.Source == _sharp) {
			acc.Source = _flat;
		}
		else if (acc.Source == _flat) {
			acc.Source = _doubleSharp;
		}
		else if (acc.Source == _doubleSharp) {
			acc.Source = _doubleFlat;
		}
		else if (acc.Source == _doubleFlat) {
			acc.Source = null;
		}
	}

	/// <summary>
	/// Obsluha kliknutí pravým tlačítkem myši – mazání noty včetně posuvek z linky/mezery.
	/// </summary>
	private void _SpaceRightClick(object sender, MouseButtonEventArgs e) 
	{
		var note = _rectangles[sender as Shapes.Rectangle];
		var head = note.Head;
		head.Visibility = Visibility.Collapsed;
		_visibleImages.Remove(head);
		var acc = note.Accidental;
		acc.Visibility = Visibility.Collapsed;
		acc.Source = null;
		_visibleImages.Remove(acc);
	}

	/// <summary>
	/// Obsluha tlačíčka "Vymazat vše" – smaže všechny viditelné noty s jejich posuvkami.
	/// </summary>
	private void _ClearAll(object sender, RoutedEventArgs e)
	{
		foreach (var img in _visibleImages) {
			img.Visibility = Visibility.Collapsed;
			img.Source = null;
		}
		_visibleImages.Clear();
	}

	/// <summary>
	/// Obsluha tlačítka "Odeslat" – připravuje uživatelem zadaný akord do vhodného tvaru a
	/// posílá ke kontrole.
	/// </summary>
	private void _Evaluate(object sender, RoutedEventArgs e)
	{
		var clicked = new List<Note>();
		foreach (var note in _notes.Where(note => note.Head.Visibility == Visibility.Visible)) {
			int acc = 0;
			if (note.Accidental.Visibility == Visibility.Visible) {
				var source = note.Accidental.Source;
				if (source == _sharp) {
					acc = 1;
				}
				else if (source == _flat) {
					acc = -1;
				}
				else if (source == _doubleSharp) {
					acc = 2;
				}
				else if (source == _doubleFlat) {
					acc = -2;
				}
			}
			string name = note.Head.Name;
			string basename = name[0].ToString();
			int octave = name.Length == 1 ? 0 : Int32.Parse(name[1].ToString());
			clicked.Add(new Note(basename, octave, acc));
		}

		_app.Evaluate(clicked);
	}

	/// <summary>
	/// Obsluha změny časomíry z pohledu GUI (mění zobrazený čas).
	/// </summary>
	public void RemainingTimeChanged(double time)
	{
		int t = (int) time;
		_countdown.Text = String.Format("{0}:{1:00}", t / 60, t % 60);
	}

	/// <summary>
	/// Obsluha změny aktuálního zadání (mění zobrazené zadání).
	/// </summary>
	public void ExcerciseChanged(string excercise) 
	{
		_ClearAll(null, null);
		_excercise.Text = excercise;
	}

	/// <summary>
	/// Obsluha změny aktuální počtu bodů (mění zobrazený počet).
	/// </summary>
	public void PointsChanged(int points)
	{
		_points.Text = String.Format("{0} b", points);
	}

	/// <summary>
	/// Zobrazení ukončovací obrazovky s dosaženým výsledkem.
	/// </summary>
	/// <param name="points">Výsledek.</param>
	public void GameOver(int points)
	{
		score.Text = points.ToString();
		_gameOver.Visibility = Visibility.Visible;
	}

	public MainWindow(App app)
	{
		_app = app;
		InitializeComponent();

		var heads = new string[] {
			"g", "a", "h",
			"c1", "d1", "e1", "f1", "g1", "a1", "h1",
			"c2", "d2", "e2", "f2", "g2", "a2", "h2",
			"c3", "d3"
		};
		foreach (var head in heads) {
			var note = new _Note() {
				Head = _canvas.FindName(head) as Controls.Image,
				Accidental = _canvas.FindName(head + "_acc") as Controls.Image
			};
			_notes.Add(note);
			_rectangles[_canvas.FindName("rec_" + head) as Shapes.Rectangle] = note;
		}
	}

	/// <summary>
	/// Inicializace hry z pohledu GUI.
	/// </summary>
	private void _Begin(object sender, RoutedEventArgs e)
	{

		var sender_btn = sender as Controls.Button;
		_intro.Visibility = System.Windows.Visibility.Collapsed;  // schování úvodní obrazovky
		if (sender_btn.Name == "beg_btn") {  // rozeznání zvolené obtížnosti a předání této informace třídě App
			_app.Begin(App.Level.Beginner);
		} else if (sender_btn.Name == "adv_btn") {
			_app.Begin(App.Level.Advanced);
		}
	}

	private void _Return(object sender, RoutedEventArgs e)
	{
		_intro.Visibility = System.Windows.Visibility.Visible;
		_gameOver.Visibility = System.Windows.Visibility.Collapsed;
	}

	private void _TerminateGame(object sender, RoutedEventArgs e)
	{
		_app.TerminateGame();
	}
}
