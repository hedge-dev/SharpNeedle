using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using SharpNeedle.IO;
using SharpNeedle.Studio.Models;

namespace SharpNeedle.Studio
{
    public class ResourceEditor
    {
        private static ConditionalWeakTable<IResource, IViewModel> mOpenEditors = new();
        private static readonly Dictionary<Type, ResourceEditorAttribute> mEditorTypes = new();

        static ResourceEditor()
        {
            RegisterAssembly(typeof(ResourceEditor).Assembly);
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            foreach (var attributePair in assembly.GetAttributedTypes<ResourceEditorAttribute>())
            {
                attributePair.Value.EditorCreator = attributePair.Key.GetAttributedFunction<ResourceEditorCreatorAttribute, Func<IResource, IViewModel>>();
                mEditorTypes.Add(attributePair.Value.Type, attributePair.Value);
            }
        }

        public static ResourceEditorAttribute GetEditorType(IResource res)
        {
            var resType = res.GetType();

            foreach (var type in mEditorTypes)
            {
                if (type.Value.Match(resType))
                    return type.Value;
            }

            return null;
        }

        public static IViewModel CreateEditor(IResource res)
        {
            if (mOpenEditors.TryGetValue(res, out var editor))
                return editor;

            editor = GetEditorType(res)?.CreateEditor(res);
            if (editor != null)
                mOpenEditors.Add(res, editor);
            
            return editor;
        }

        public static IViewModel OpenEditor(IFile file)
        {
            return OpenEditor(Singleton.GetInstance<IResourceManager>().Open(file));
        }

        public static IViewModel OpenEditor(IResource resource)
        {
            if (mOpenEditors.TryGetValue(resource, out var editor))
            {
                Singleton.GetInstance<Workspace>().SelectedDocument = editor;
                return editor;
            }
            editor = CreateEditor(resource);
            if (editor == null)
            {
                Singleton.GetInstance<IResourceManager>().Close(resource);
                return null;
            }

            Singleton.GetInstance<Workspace>().AddDocument(editor);
            return editor;
        }
    }
}
