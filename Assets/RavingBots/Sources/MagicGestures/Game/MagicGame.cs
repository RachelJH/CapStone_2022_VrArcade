using System.Collections;

using RavingBots.MagicGestures.AI;
using RavingBots.MagicGestures.Game.Magic;
using RavingBots.MagicGestures.UI;
using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.ObjectPooling;

using UnityEngine;

namespace RavingBots.MagicGestures.Game
{
	/// <summary>
	///     The main game configuration.
	/// </summary>
	/// <remarks>
	///     This component contains the main configuration of the game,
	///     as well as the runtime object pools for the spell effects.
	/// </remarks>
	public class MagicGame : MonoSingleton<MagicGame>
	{
		/// <summary>
		///     The number of configured spells.
		/// </summary>
		public int EffectCount
		{
			get { return MagicEffectPools.Length; }
		}

		/// <summary>
		///     The prefab containing the network and configured gestures.
		/// </summary>
		[SerializeField]
		protected GameData GameData;
		/// <summary>
		///     If <see langword="true" />, <see cref="GameData" /> will be saved
		///     when the game quits.
		/// </summary>
		[SerializeField]
		protected bool SaveOnQuit;

		/// <summary>
		///     The spell effect prefabs.
		/// </summary>
		[SerializeField]
		protected MagicEffect[] MagicEffectPrefabs;
		/// <summary>
		///     The pools of the spell effect objects.
		/// </summary>
		protected GameObjectPool<MagicEffect>[] MagicEffectPools { get; private set; }

		/// <summary>
		///     The number of spell effect objects cached when a pool
		///     is created.
		/// </summary>
		private const int Precache = 5;

		/// <summary>
		///     Initialize the object pools.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			MagicEffectPools = new GameObjectPool<MagicEffect>[MagicEffectPrefabs.Length];

			for (var i = 0; i < MagicEffectPools.Length; i++)
				MagicEffectPools[i] = new GameObjectPool<MagicEffect>(MagicEffectPrefabs[i], Precache);
		}

		/// <summary>
		///     Load the spell data and show the welcome screen.
		/// </summary>
		protected void Start()
		{
			LoadData();

			StartCoroutine(ShowWelcome());
		}

		/// <summary>
		///     Show the welcome screen after a delay (coroutine).
		/// </summary>
		private IEnumerator ShowWelcome()
		{
			yield return new WaitForSeconds(1f);

			var menu = MagicMenu.Instance;

			menu.CurrentView = menu.WelcomeView;
			menu.transform.position = menu.WelcomeView.Position;
			menu.transform.rotation = Quaternion.identity;
		}

		/// <summary>
		///     Get a random spell index.
		/// </summary>
		public int GetRandomEffectId()
		{
			return Random.Range(0, MagicEffectPools.Length);
		}

		/// <summary>
		///     Get a spell effect object.
		/// </summary>
		/// <param name="id">The index of the spell.</param>
		/// <returns>An effect object from the appropriate pool.</returns>
		public MagicEffect CreateMagicEffect(int id)
		{
			if ((id < 0) || (id >= EffectCount))
				return null;

			return MagicEffectPools[id].TakeInstance();
		}

		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Quit();
		}

		/// <summary>
		///     Load the data from the <c>ScriptableObject</c> asset into
		///     the game components.
		/// </summary>
		protected void LoadData()
		{
			GestureLearner.Instance.MLP = GameData.MLP;
			StartCoroutine(WaitMenu());
		}

		/// <summary>
		///     Assign spells in Spell Book View after Magic Menu
		///     singleton is loaded and ready.
		/// </summary>
		IEnumerator WaitMenu()
		{
			yield return new WaitUntil(() => MagicMenu.Instance != null);
			MagicMenu.Instance.SpellBookView.Spells = GameData.Spells;
		}

		/// <summary>
		///     Save the current data into an asset.
		/// </summary>
		protected void SaveData()
		{
			var learner = GestureLearner.Instance;

			if (!learner.Status.IsRunning)
			{
				GameData.MLP = learner.MLP;
				GameData.Spells = MagicMenu.Instance.SpellBookView.Spells;

				GameData.Save();
			}
			else
				Debug.Log("Cannot save while training is still running.");
		}

		/// <summary>
		///     Quit the game.
		/// </summary>
		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		/// <summary>
		///     Trigger an autosave if <see cref="SaveOnQuit" /> is set.
		/// </summary>
		protected void OnApplicationQuit()
		{
			if (SaveOnQuit)
				SaveData();
		}
	}
}
