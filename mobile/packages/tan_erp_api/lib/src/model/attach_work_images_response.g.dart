// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attach_work_images_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachWorkImagesResponse extends AttachWorkImagesResponse {
  @override
  final BuiltList<OpportunityWorkImageResponse>? items;
  @override
  final String? opportunityRowVersion;

  factory _$AttachWorkImagesResponse(
          [void Function(AttachWorkImagesResponseBuilder)? updates]) =>
      (AttachWorkImagesResponseBuilder()..update(updates))._build();

  _$AttachWorkImagesResponse._({this.items, this.opportunityRowVersion})
      : super._();
  @override
  AttachWorkImagesResponse rebuild(
          void Function(AttachWorkImagesResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachWorkImagesResponseBuilder toBuilder() =>
      AttachWorkImagesResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachWorkImagesResponse &&
        items == other.items &&
        opportunityRowVersion == other.opportunityRowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, opportunityRowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachWorkImagesResponse')
          ..add('items', items)
          ..add('opportunityRowVersion', opportunityRowVersion))
        .toString();
  }
}

class AttachWorkImagesResponseBuilder
    implements
        Builder<AttachWorkImagesResponse, AttachWorkImagesResponseBuilder> {
  _$AttachWorkImagesResponse? _$v;

  ListBuilder<OpportunityWorkImageResponse>? _items;
  ListBuilder<OpportunityWorkImageResponse> get items =>
      _$this._items ??= ListBuilder<OpportunityWorkImageResponse>();
  set items(ListBuilder<OpportunityWorkImageResponse>? items) =>
      _$this._items = items;

  String? _opportunityRowVersion;
  String? get opportunityRowVersion => _$this._opportunityRowVersion;
  set opportunityRowVersion(String? opportunityRowVersion) =>
      _$this._opportunityRowVersion = opportunityRowVersion;

  AttachWorkImagesResponseBuilder() {
    AttachWorkImagesResponse._defaults(this);
  }

  AttachWorkImagesResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _opportunityRowVersion = $v.opportunityRowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachWorkImagesResponse other) {
    _$v = other as _$AttachWorkImagesResponse;
  }

  @override
  void update(void Function(AttachWorkImagesResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachWorkImagesResponse build() => _build();

  _$AttachWorkImagesResponse _build() {
    _$AttachWorkImagesResponse _$result;
    try {
      _$result = _$v ??
          _$AttachWorkImagesResponse._(
            items: _items?.build(),
            opportunityRowVersion: opportunityRowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachWorkImagesResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
