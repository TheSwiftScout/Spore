using FluentAssertions;
using Spore.CrossSeed;

namespace Spore.Tests;

public class PieceSizeSelectionAlgorithmTests
{
    [Fact]
    public void Test_min_return_value_of_all_algorithms()
    {
        PieceSizeSelectionAlgorithms
            .IntermodalImproved(long.MinValue)
            .Should()
            .Be(PieceSizeSelectionAlgorithms.MinPieceSize);

        PieceSizeSelectionAlgorithms
            .UpperBoundPieceCount(long.MinValue)
            .Should()
            .Be(PieceSizeSelectionAlgorithms.MinPieceSize);
    }

    [Fact]
    public void Test_max_return_value_of_all_algorithms()
    {
        PieceSizeSelectionAlgorithms
            // TODO fix the following
            .IntermodalImproved(long.MaxValue)
            .IntermodalImproved(2L * 1_073_741_824 * 1024)
            .Should()
            .Be(PieceSizeSelectionAlgorithms.MaxPieceSize);

        PieceSizeSelectionAlgorithms
            .UpperBoundPieceCount(long.MaxValue)
            .Should()
            .Be(PieceSizeSelectionAlgorithms.MaxPieceSize);
    }

    // Acceptance criteria for Intermodal's algorithm
    // Spec: https://imdl.io/book/bittorrent/piece-length-selection.html#intermodals-algorithm
    [Theory]
    // 1 KiB - 512 KiB
    [InlineData(16, 16)]
    [InlineData(32, 16)]
    [InlineData(64, 16)]
    [InlineData(128, 16)]
    [InlineData(256, 16)]
    [InlineData(512, 16)]
    // 1 MiB - 512 MiB
    [InlineData(1 * 1024, 16)]
    [InlineData(2 * 1024, 16)]
    [InlineData(4 * 1024, 32)]
    [InlineData(8 * 1024, 32)]
    [InlineData(16 * 1024, 64)]
    [InlineData(32 * 1024, 64)]
    [InlineData(64 * 1024, 128)]
    [InlineData(128 * 1024, 128)]
    [InlineData(256 * 1024, 256)]
    [InlineData(512 * 1024, 256)]
    // 1 GiB - 512 GiB
    [InlineData(1 * 1_048_576, 512)]
    [InlineData(2 * 1_048_576, 512)]
    [InlineData(4 * 1_048_576, 1024)]
    [InlineData(8 * 1_048_576, 1024)]
    [InlineData(16 * 1_048_576, 2048)]
    [InlineData(32 * 1_048_576, 2048)]
    [InlineData(64 * 1_048_576, 4096)]
    [InlineData(128 * 1_048_576, 4096)]
    [InlineData(256 * 1_048_576, 8192)]
    [InlineData(512 * 1_048_576, 8192)]
    // 1 TiB - 512 TiB
    [InlineData(1L * 1_073_741_824, 16384)]
    [InlineData(2L * 1_073_741_824, 16384)]
    [InlineData(4L * 1_073_741_824, 16384)]
    [InlineData(8L * 1_073_741_824, 16384)]
    [InlineData(16L * 1_073_741_824, 16384)]
    [InlineData(32L * 1_073_741_824, 16384)]
    [InlineData(64L * 1_073_741_824, 16384)]
    [InlineData(128L * 1_073_741_824, 16384)]
    [InlineData(256L * 1_073_741_824, 16384)]
    [InlineData(512L * 1_073_741_824, 16384)]
    // 1 PiB
    [InlineData(1_099_511_627_776, 16384)]
    public void IntermodalImprovedTest(long contentSizeKib, int expectedPieceSizeKib)
    {
        var actualPieceSize = PieceSizeSelectionAlgorithms.IntermodalImproved(contentSizeKib * 1024);
        var actualPieceSizeKib = actualPieceSize / 1024;
        actualPieceSizeKib.Should().Be(expectedPieceSizeKib);
    }
}
