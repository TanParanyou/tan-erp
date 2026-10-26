// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetLineResponse extends ProjectBudgetLineResponse {
  @override
  final String? id;
  @override
  final String? category;
  @override
  final String? description;
  @override
  final double? amount;
  @override
  final int? sortOrder;

  factory _$ProjectBudgetLineResponse(
          [void Function(ProjectBudgetLineResponseBuilder)? updates]) =>
      (ProjectBudgetLineResponseBuilder()..update(updates))._build();

  _$ProjectBudgetLineResponse._(
      {this.id, this.category, this.description, this.amount, this.sortOrder})
      : super._();
  @override
  ProjectBudgetLineResponse rebuild(
          void Function(ProjectBudgetLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetLineResponseBuilder toBuilder() =>
      ProjectBudgetLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetLineResponse &&
        id == other.id &&
        category == other.category &&
        description == other.description &&
        amount == other.amount &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetLineResponse')
          ..add('id', id)
          ..add('category', category)
          ..add('description', description)
          ..add('amount', amount)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class ProjectBudgetLineResponseBuilder
    implements
        Builder<ProjectBudgetLineResponse, ProjectBudgetLineResponseBuilder> {
  _$ProjectBudgetLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _category;
  String? get category => _$this._category;
  set category(String? category) => _$this._category = category;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  ProjectBudgetLineResponseBuilder() {
    ProjectBudgetLineResponse._defaults(this);
  }

  ProjectBudgetLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _category = $v.category;
      _description = $v.description;
      _amount = $v.amount;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetLineResponse other) {
    _$v = other as _$ProjectBudgetLineResponse;
  }

  @override
  void update(void Function(ProjectBudgetLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetLineResponse build() => _build();

  _$ProjectBudgetLineResponse _build() {
    final _$result = _$v ??
        _$ProjectBudgetLineResponse._(
          id: id,
          category: category,
          description: description,
          amount: amount,
          sortOrder: sortOrder,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
