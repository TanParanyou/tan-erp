// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetLineRequest extends ProjectBudgetLineRequest {
  @override
  final String? category;
  @override
  final String? description;
  @override
  final double? amount;

  factory _$ProjectBudgetLineRequest(
          [void Function(ProjectBudgetLineRequestBuilder)? updates]) =>
      (ProjectBudgetLineRequestBuilder()..update(updates))._build();

  _$ProjectBudgetLineRequest._({this.category, this.description, this.amount})
      : super._();
  @override
  ProjectBudgetLineRequest rebuild(
          void Function(ProjectBudgetLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetLineRequestBuilder toBuilder() =>
      ProjectBudgetLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetLineRequest &&
        category == other.category &&
        description == other.description &&
        amount == other.amount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetLineRequest')
          ..add('category', category)
          ..add('description', description)
          ..add('amount', amount))
        .toString();
  }
}

class ProjectBudgetLineRequestBuilder
    implements
        Builder<ProjectBudgetLineRequest, ProjectBudgetLineRequestBuilder> {
  _$ProjectBudgetLineRequest? _$v;

  String? _category;
  String? get category => _$this._category;
  set category(String? category) => _$this._category = category;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  ProjectBudgetLineRequestBuilder() {
    ProjectBudgetLineRequest._defaults(this);
  }

  ProjectBudgetLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _category = $v.category;
      _description = $v.description;
      _amount = $v.amount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetLineRequest other) {
    _$v = other as _$ProjectBudgetLineRequest;
  }

  @override
  void update(void Function(ProjectBudgetLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetLineRequest build() => _build();

  _$ProjectBudgetLineRequest _build() {
    final _$result = _$v ??
        _$ProjectBudgetLineRequest._(
          category: category,
          description: description,
          amount: amount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
