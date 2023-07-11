using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JasperFx.Core;
using Marten.Linq.Parsing.Operators;
using Marten.Schema;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.Members;

internal class DocumentQueryableMemberCollection: IQueryableMemberCollection, IQueryableMember
{
    private readonly StoreOptions _options;
    private ImHashMap<string, IQueryableMember> _members = ImHashMap<string, IQueryableMember>.Empty;

    public DocumentQueryableMemberCollection(IDocumentMapping mapping, StoreOptions options)
    {
        _options = options;
        ElementType = mapping.DocumentType;
    }

    public string MemberName => string.Empty;

    public string JsonPathSegment => "";

    void ISqlFragment.Apply(CommandBuilder builder)
    {
        builder.Append("d.data");
    }

    bool ISqlFragment.Contains(string sqlText)
    {
        return false;
    }

    Type IQueryableMember.MemberType => ElementType;

    string IQueryableMember.TypedLocator => "d.data";

    string IQueryableMember.RawLocator => "d.data";

    string IQueryableMember.JSONBLocator => "d.data";

    string IQueryableMember.BuildOrderingExpression(Ordering ordering, CasingRule casingRule)
    {
        throw new NotSupportedException();
    }

    IQueryableMember[] IQueryableMember.Ancestors => Array.Empty<IQueryableMember>();

    Dictionary<string, object> IQueryableMember.FindOrPlaceChildDictionaryForContainment(
        Dictionary<string, object> dict)
    {
        throw new NotSupportedException();
    }

    void IQueryableMember.PlaceValueInDictionaryForContainment(Dictionary<string, object> dict,
        ConstantExpression constant)
    {
        throw new NotSupportedException();
    }

    string IQueryableMember.LocatorForIncludedDocumentId => throw new NotSupportedException();
    public string NullTestLocator { get; } = "d.data";

    public string SelectorForDuplication(string pgType)
    {
        throw new NotSupportedException();
    }

    public Type ElementType { get; }

    public IQueryableMember FindMember(MemberInfo member)
    {
        if (_members.TryFind(member.Name, out var m))
        {
            return m;
        }

        m = _options.CreateQueryableMember(member, this);
        _members = _members.AddOrUpdate(member.Name, m);

        return m;
    }

    public void ReplaceMember(MemberInfo member, IQueryableMember queryableMember)
    {
        _members = _members.AddOrUpdate(member.Name, queryableMember);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<IQueryableMember> GetEnumerator()
    {
        return _members.Enumerate().Select(x => x.Value).GetEnumerator();
    }
}
