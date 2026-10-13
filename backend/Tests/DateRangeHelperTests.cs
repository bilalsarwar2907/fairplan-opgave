using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Common;

namespace Tests
{
    public class DateRangeHelperTests
    {
        private static DateOnly D(int y, int m, int d) => new(y, m, d);

        [Fact]
        public void Overlapping_ranges_return_true() =>
            Assert.True(DateRangeHelper.Overlaps(D(2026, 12, 1), D(2026, 12, 31), D(2026, 12, 20), D(2027, 1, 10)));

        [Fact]
        public void Adjacent_ranges_return_false() =>        // 31 Dec and 1 Jan touch but don't overlap
            Assert.False(DateRangeHelper.Overlaps(D(2026, 12, 1), D(2026, 12, 31), D(2027, 1, 1), D(2027, 1, 31)));

        [Fact]
        public void Sharing_one_day_returns_true() =>        // inclusive: 31 Dec is in both
            Assert.True(DateRangeHelper.Overlaps(D(2026, 12, 1), D(2026, 12, 31), D(2026, 12, 31), D(2027, 1, 15)));

        [Fact]
        public void Separate_ranges_return_false() =>
            Assert.False(DateRangeHelper.Overlaps(D(2026, 12, 1), D(2026, 12, 10), D(2026, 12, 20), D(2026, 12, 31)));
    }
}