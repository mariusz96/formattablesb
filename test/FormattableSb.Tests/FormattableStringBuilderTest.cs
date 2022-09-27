using System;
using System.Collections.Generic;
using Xunit;

namespace FormattableSb.Tests
{
    public class FormattableStringBuilderTest
    {
        [Fact]
        public void AppendInterpolated_Empty()
        {
            var fsb = new FormattableStringBuilder();

            var fs = fsb.ToFormattableString();

            Assert.Equal(string.Empty, fs.Format);
            Assert.Equal(0, fs.ArgumentCount);
        }

        [Theory]
        [MemberData(nameof(AppendInterpolated_Single_Data))]
        public void AppendInterpolated_Single(object[] args)
        {
            var fsb = new FormattableStringBuilder();

            var fs = fsb
                .AppendInterpolated($"{args[0]}")
                .ToFormattableString();

            Assert.Equal("{0}", fs.Format);
            Assert.Equal(args, fs.GetArguments());
        }

        public static IEnumerable<object[]> AppendInterpolated_Single_Data =>
            new List<object[]>
            {
                new object[] { new object[] { "one" } },
                new object[] { new object[] { new FormattableStringBuilder() } }
            };


        [Theory]
        [MemberData(nameof(AppendInterpolated_Multiple_Data))]
        public void AppendInterpolated_Multiple(object[] args)
        {
            var fsb = new FormattableStringBuilder();

            var fs = fsb
                .AppendInterpolated($"{args[0]}")
                .AppendInterpolated($"{args[1]}")
                .ToFormattableString();

            Assert.Equal("{0}{1}", fs.Format);
            Assert.Equal(args, fs.GetArguments());
        }

        public static IEnumerable<object[]> AppendInterpolated_Multiple_Data =>
            new List<object[]>
            {
                new object[] { new object[] { "one", "two" } },
                new object[] { new object[] { new FormattableStringBuilder(), new FormattableStringBuilder() } }
            };

        [Fact]
        public void AppendInterpolated_Braces()
        {
            var fsb = new FormattableStringBuilder();
            var args = new[] { "two", "four" };

            var fs = fsb
                .AppendInterpolated($"{{one}} {{{args[0]}}} {{{{three}}}} {{{{{args[1]}}}}}")
                .ToFormattableString();

            Assert.Equal("{{one}} {{{0}}} {{{{three}}}} {{{{{1}}}}}", fs.Format);
            Assert.Equal(args, fs.GetArguments());
        }

        [Fact]
        public void AppendInterpolated_Readme()
        {
            var firstDayOfSummer = new DateTime(2022, 6, 21);
            var lastDayOfSummer = new DateTime(2022, 6, 22);

            var sqlBuilder = new FormattableStringBuilder()
                .AppendInterpolated($"INSERT INTO dbo.VacationDates (Date)")
                .AppendLine()
                .AppendInterpolated($"VALUES");

            for (var date = firstDayOfSummer; date <= lastDayOfSummer; date = date.AddDays(1))
            {
                sqlBuilder
                    .AppendLine()
                    .AppendInterpolated($"({date})");

                if (date != lastDayOfSummer)
                {
                    sqlBuilder.AppendInterpolated($",");
                }
            }

            var sql = sqlBuilder.ToFormattableString();

            Assert.Equal(
@"INSERT INTO dbo.VacationDates (Date)
VALUES
({0}),
({1})", sql.Format);
            Assert.Equal(new object[] { firstDayOfSummer, lastDayOfSummer }, sql.GetArguments());
        }
    }
}