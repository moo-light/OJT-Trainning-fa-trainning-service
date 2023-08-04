using Application.Utils;
using BCrypt.Net;
using Castle.Components.DictionaryAdapter.Xml;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using System.ComponentModel;
using BCrypty = BCrypt.Net.BCrypt;

namespace Application.Tests.Utils
{
    /// <class name="StringUtils.cs"></class>
    public class StringUtilsTests : SetupTest
    {
        [Fact]
        public void Hash_ShouldReturnHashedCharacter()
        {
            // Setup
            string password = "password"; //very common password
            // Act
            var result = StringUtils.Hash(password);
            // Assert
            result.Should().NotBe(password);
            BCrypty.Verify(password, result).Should().BeTrue();
        }
        [Fact]
        public void CheckPasswordShouldReturnTrue()
        {
            // Setup
            string password = "password"; //very common password
            string hashPassword = password.Hash();
            // Act
            var result = StringUtils.CheckPassword(password, hashPassword);
            // Assert
            result.Should().BeTrue();
            BCrypty.Verify(password, hashPassword).Should().Be(result);

        }
        [Fact]
        public void ThrowErrorIfNotValidEnum_ShouldReturnTrue()
        {
            // Setup
            string right_enum_1 = nameof(AttendanceStatusEnums.None).ToLower();
            string right_enum_2 = nameof(AttendanceStatusEnums.Absent).ToUpper();
            string right_enum_3 = nameof(AttendanceStatusEnums.Present);
            string right_enum_4 = nameof(AttendanceStatusEnums.AbsentPermit);
            // Act and Assert
            StringUtils.ThrowErrorIfNotValidEnum(right_enum_1, typeof(AttendanceStatusEnums)).Should().BeTrue();
            StringUtils.ThrowErrorIfNotValidEnum(right_enum_2, typeof(AttendanceStatusEnums)).Should().BeTrue();
            StringUtils.ThrowErrorIfNotValidEnum(right_enum_3, typeof(AttendanceStatusEnums)).Should().BeTrue();
            StringUtils.ThrowErrorIfNotValidEnum(right_enum_4, typeof(AttendanceStatusEnums)).Should().BeTrue();
        }
        [Fact]
        public void ThrowErrorIfNotValidEnum_ShouldThrowError()
        {
            // Setup
            string this_should_throw_error = "anything goes";
            Type type = typeof(AttendanceStatusEnums);
            string message = "throw error should contain this";
            // Act
            Func<bool> result = () => this_should_throw_error.ThrowErrorIfNotValidEnum(type);
            Func<bool> result_message = () => this_should_throw_error.ThrowErrorIfNotValidEnum(type, message);
            // Assert
            result.Should().ThrowExactly<InvalidEnumArgumentException>("because the enum is not valid");
            result.Should().Throw<InvalidEnumArgumentException>().WithMessage($"*Try using*{string.Join(", ", Enum.GetNames(type))}*");
            result_message.Should().Throw<InvalidEnumArgumentException>().WithMessage($"*{message}*");
        }
    }
}
