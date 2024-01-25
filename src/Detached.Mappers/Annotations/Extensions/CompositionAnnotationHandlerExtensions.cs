﻿using Detached.Mappers.Annotations;
using Detached.Mappers.TypePairs;
using Detached.Mappers.TypePairs.Builder;
using Detached.Mappers.Types;
using Detached.Mappers.Types.Class.Builder;

namespace Detached.Mappers
{
    public static class CompositionAnnotationHandlerExtensions
    {
        public static Annotation<bool> Composition(this AnnotationCollection annotations)
        {
            return annotations.Annotation<bool>("DETACHED_COMPOSITION");
        }

        public static ITypeMember Composition(this ITypeMember member, bool value)
        {
            member.Annotations.Composition().Set(value);

            return member;
        }

        public static ClassTypeMemberBuilder<TType, TMember> Composition<TType, TMember>(this ClassTypeMemberBuilder<TType, TMember> memberBuilder, bool value = true)
        {
            memberBuilder.Member.Annotations.Composition().Set(value);

            return memberBuilder;
        }

        public static TypePairMember Composition(this TypePairMember member, bool value = true)
        {
            member.Annotations.Composition().Set(value);

            return member;
        }

        public static TypePairMemberBuilder<TType, TMember> Composition<TType, TMember>(this TypePairMemberBuilder<TType, TMember> memberBuilder, bool value = true)
        {
            memberBuilder.Member.Annotations.Composition().Set(value);

            return memberBuilder;
        }
    }
}