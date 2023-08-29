using System;

namespace Spore.CrossSeed;

/// <summary>
///     Torrent piece size selection algorithms.
/// </summary>
public static class PieceSizeSelectionAlgorithms
{
    /// <summary>
    ///     Minimum piece size that any algorithm returns: 16 KiB
    /// </summary>
    public const int MinPieceSize = 16_384;

    /// <summary>
    ///     Maximum piece size that any algorithm returns: 16 MiB
    /// </summary>
    public const int MaxPieceSize = 16_777_216;

    /// <summary>
    ///     Improved intermodal piece size selection algorithm.
    ///     License:
    ///     CC0 1.0 Universal
    ///     https://creativecommons.org/publicdomain/zero/1.0/
    ///     Basic Implementation:
    ///     https://github.com/casey/intermodal/blob/68012b1408778263c6a1aa27fa60729c1d125d4d/src/piece_length_picker.rs
    ///     The following GitHub issue gives a good overview of how other common torrent file tools select piece sizes:
    ///     https://github.com/casey/intermodal/issues/367
    /// </summary>
    /// <param name="contentSizeBytes">Content size.</param>
    /// <returns>The calculated piece size.</returns>
    public static int IntermodalImproved(long contentSizeBytes)
    {
        var leftShiftBy = Math.Ceiling(Math.Log2(contentSizeBytes));
        var pieceSize = 16 << ((int)leftShiftBy / 2);
        return Math.Min(
            Math.Max(pieceSize, MinPieceSize),
            MaxPieceSize);
    }

    /// <summary>
    ///     Piece size selection algorithm with fixed upper bound of piece count.
    /// </summary>
    /// <param name="contentSizeBytes">Content size.</param>
    /// <param name="maxPiecesCount">Max count of pieces; defaults to 2200</param>
    /// <returns>The calculated piece size.</returns>
    public static int UpperBoundPieceCount(long contentSizeBytes, int maxPiecesCount = 2200)
    {
        for (var pieceSize = MinPieceSize; pieceSize < MaxPieceSize; pieceSize *= 2)
        {
            var piecesCount = contentSizeBytes / pieceSize + 1;
            if (piecesCount <= maxPiecesCount)
                return pieceSize;
        }

        return MaxPieceSize;
    }
}
