﻿using System;

namespace EntityFramework.Toolkit.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToKind(this DateTime value, DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    return value.SpecifyKind(kind);

                case DateTimeKind.Utc:
                    return value.ToKindUtc();

                case DateTimeKind.Local:
                    return value.ToKindLocal();
   ;         }

            throw new InvalidOperationException($"Cannot convert to DateTimeKind.{kind}");
        }

        public static DateTime ToKindUtc(this DateTime value)
        {
            return KindUtc(value);
        }

        public static DateTime? ToKindUtc(this DateTime? value)
        {
            return KindUtc(value);
        }

        public static DateTime ToKindLocal(this DateTime value)
        {
            return KindLocal(value);
        }

        public static DateTime? ToKindLocal(this DateTime? value)
        {
            return KindLocal(value);
        }

        public static DateTime SpecifyKind(this DateTime value, DateTimeKind kind)
        {
            if (value.Kind != kind)
            {
                return DateTime.SpecifyKind(value, kind);
            }
            return value;
        }

        public static DateTime? SpecifyKind(this DateTime? value, DateTimeKind kind)
        {
            if (value.HasValue)
            {
                return DateTime.SpecifyKind(value.Value, kind);
            }
            return null;
        }

        public static DateTime KindUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
            if (value.Kind == DateTimeKind.Local)
            {
                return value.ToUniversalTime();
            }
            return value;
        }

        public static DateTime? KindUtc(DateTime? value)
        {
            if (value.HasValue)
            {
                return KindUtc(value.Value);
            }
            return null;
        }

        public static DateTime KindLocal(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(value, DateTimeKind.Local);
            }

            if (value.Kind == DateTimeKind.Utc)
            {
                return value.ToLocalTime();
            }
            return value;
        }

        public static DateTime? KindLocal(DateTime? value)
        {
            if (value.HasValue)
            {
                return KindLocal(value.Value);
            }
            return null;
        }
    }
}