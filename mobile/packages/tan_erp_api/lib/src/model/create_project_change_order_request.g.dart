// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_project_change_order_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateProjectChangeOrderRequest
    extends CreateProjectChangeOrderRequest {
  @override
  final String? title;
  @override
  final String? reason;
  @override
  final double? budgetDelta;
  @override
  final double? contractDelta;

  factory _$CreateProjectChangeOrderRequest(
          [void Function(CreateProjectChangeOrderRequestBuilder)? updates]) =>
      (CreateProjectChangeOrderRequestBuilder()..update(updates))._build();

  _$CreateProjectChangeOrderRequest._(
      {this.title, this.reason, this.budgetDelta, this.contractDelta})
      : super._();
  @override
  CreateProjectChangeOrderRequest rebuild(
          void Function(CreateProjectChangeOrderRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateProjectChangeOrderRequestBuilder toBuilder() =>
      CreateProjectChangeOrderRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateProjectChangeOrderRequest &&
        title == other.title &&
        reason == other.reason &&
        budgetDelta == other.budgetDelta &&
        contractDelta == other.contractDelta;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, budgetDelta.hashCode);
    _$hash = $jc(_$hash, contractDelta.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateProjectChangeOrderRequest')
          ..add('title', title)
          ..add('reason', reason)
          ..add('budgetDelta', budgetDelta)
          ..add('contractDelta', contractDelta))
        .toString();
  }
}

class CreateProjectChangeOrderRequestBuilder
    implements
        Builder<CreateProjectChangeOrderRequest,
            CreateProjectChangeOrderRequestBuilder> {
  _$CreateProjectChangeOrderRequest? _$v;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  double? _budgetDelta;
  double? get budgetDelta => _$this._budgetDelta;
  set budgetDelta(double? budgetDelta) => _$this._budgetDelta = budgetDelta;

  double? _contractDelta;
  double? get contractDelta => _$this._contractDelta;
  set contractDelta(double? contractDelta) =>
      _$this._contractDelta = contractDelta;

  CreateProjectChangeOrderRequestBuilder() {
    CreateProjectChangeOrderRequest._defaults(this);
  }

  CreateProjectChangeOrderRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _title = $v.title;
      _reason = $v.reason;
      _budgetDelta = $v.budgetDelta;
      _contractDelta = $v.contractDelta;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateProjectChangeOrderRequest other) {
    _$v = other as _$CreateProjectChangeOrderRequest;
  }

  @override
  void update(void Function(CreateProjectChangeOrderRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateProjectChangeOrderRequest build() => _build();

  _$CreateProjectChangeOrderRequest _build() {
    final _$result = _$v ??
        _$CreateProjectChangeOrderRequest._(
          title: title,
          reason: reason,
          budgetDelta: budgetDelta,
          contractDelta: contractDelta,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
