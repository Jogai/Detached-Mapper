﻿using Detached.Mappers.Annotations;
using Detached.Mappers.Types;
using Detached.Mappers.Types.Class.Builder;

namespace Detached.Mappers
{
    public static class AbstractAnnotationHandlerExtensions
    {
        public static Annotation<bool> Abstract(this AnnotationCollection annotations)
        {
            return annotations.Annotation<bool>("DETACHED_ABSTRACT");
        }

        public static IType Abstract(this IType type, bool value = true)
        {
            type.Annotations.Abstract().Set(value);

            return type;
        }

        public static ClassTypeBuilder<TType> Abstract<TType>(this ClassTypeBuilder<TType> typeBuilder, bool value = true)
        {
            typeBuilder.Type.Abstract(value);

            return typeBuilder;
        }
    }
}