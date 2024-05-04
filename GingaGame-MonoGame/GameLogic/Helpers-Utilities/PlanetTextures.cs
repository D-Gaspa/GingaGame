using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Static class that manages the textures of different planet types, ensuring each texture is loaded only once and
///     reused when needed.
/// </summary>
public static class PlanetTextures
{
    /// <summary>
    ///     Stores the PlanetType to Texture2D mapping.
    /// </summary>
    private static readonly Dictionary<PlanetType, Texture2D> CachedTextures = new();

    private static ContentManager _contentManager;

    /// <summary>
    ///     Sets the content manager for loading the textures.
    /// </summary>
    /// <param name="manager">The ContentManager instance to use for loading textures.</param>
    public static void SetContentManager(ContentManager manager)
    {
        _contentManager = manager;
    }

    /// <summary>
    ///     Initializes the texture cache for each planet.
    /// </summary>
    public static void InitializePlanetTextures()
    {
        foreach (var planetType in Enum.GetValues<PlanetType>()) GetCachedTexture(planetType);
    }

    /// <summary>
    ///     Retrieves the cached texture for a specific planet type, or loads it into cache if not already done.
    /// </summary>
    /// <param name="planetType">The type of the planet whose texture is needed.</param>
    /// <returns>The Texture2D instance for the specified PlanetType.</returns>
    public static Texture2D GetCachedTexture(PlanetType planetType)
    {
        if (CachedTextures.TryGetValue(planetType, out var texture)) return texture;

        // Load and cache the texture
        var textureName = GetTextureName(planetType);

        CachedTextures[planetType] = _contentManager.Load<Texture2D>("Resources/" + textureName);

        return CachedTextures[planetType];
    }

    /// <summary>
    ///     Returns the filename of the texture for a specific planet type.
    /// </summary>
    /// <param name="planetType">The type of the planet whose texture filename is needed.</param>
    /// <returns>The filename of the Texture2D for the specified PlanetType.</returns>
    private static string GetTextureName(PlanetType planetType)
    {
        return planetType.ToString();
    }
}