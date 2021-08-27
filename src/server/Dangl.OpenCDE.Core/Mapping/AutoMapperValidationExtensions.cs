using AutoMapper;
using System;
using System.Linq;

namespace Dangl.OpenCDE.Core.Mapping
{
    /// <summary>
    /// This class provides extensions to validate AutoMapper configurations
    /// </summary>
    public static class AutoMapperValidationExtensions
    {
        /// <summary>
        /// This will add a validator that checks enum types are compatible with each other
        /// </summary>
        /// <param name="mapperConfig"></param>
        /// <returns></returns>
        public static IMapperConfigurationExpression AddEnumCompatibilityValidator(this IMapperConfigurationExpression mapperConfig)
        {
            // The following checks that all enums have matching source and destination types
            mapperConfig.Advanced.Validator(c =>
            {
                if (c.Types.SourceType.IsEnum && (c.TypeMap == null || c.TypeMap.ConfiguredMemberList == MemberList.Source))
                {
                    CheckEnumsCompatible(c.Types.SourceType, c.Types.DestinationType);
                }

                if (c.Types.DestinationType.IsEnum && (c.TypeMap == null || c.TypeMap.ConfiguredMemberList == MemberList.Destination))
                {
                    CheckEnumsCompatible(c.Types.DestinationType, c.Types.SourceType);
                }
            });
            return mapperConfig;
        }

        private static void CheckEnumsCompatible(Type sourceType, Type destinationType)
        {
            var sourceValues = Enum.GetValues(sourceType).Cast<object>().Select(e => e.ToString());
            if (destinationType == typeof(string))
            {
                // Mapping to string is valid from enum
                return;
            }
            var destValues = Enum.GetValues(destinationType).Cast<object>().Select(e => e.ToString()).ToList();

            foreach (var sourceValue in sourceValues)
            {
                var destinationContainsSource = destValues.Select(e => e.ToUpperInvariant()).Contains(sourceValue.ToUpperInvariant());
                if (!destinationContainsSource)
                {
                    throw new Exception("Enum types not compatible."
                                        + Environment.NewLine + $"Source: {sourceType.Name}, Destination: {destinationType.Name}"
                                        + Environment.NewLine + $"Destination has no value for {sourceValue}");
                }
            }
        }
    }
}
