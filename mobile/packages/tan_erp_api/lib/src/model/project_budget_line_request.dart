//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_line_request.g.dart';

/// ProjectBudgetLineRequest
///
/// Properties:
/// * [category] 
/// * [description] 
/// * [amount] 
@BuiltValue()
abstract class ProjectBudgetLineRequest implements Built<ProjectBudgetLineRequest, ProjectBudgetLineRequestBuilder> {
  @BuiltValueField(wireName: r'category')
  String? get category;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  ProjectBudgetLineRequest._();

  factory ProjectBudgetLineRequest([void updates(ProjectBudgetLineRequestBuilder b)]) = _$ProjectBudgetLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetLineRequest> get serializer => _$ProjectBudgetLineRequestSerializer();
}

class _$ProjectBudgetLineRequestSerializer implements PrimitiveSerializer<ProjectBudgetLineRequest> {
  @override
  final Iterable<Type> types = const [ProjectBudgetLineRequest, _$ProjectBudgetLineRequest];

  @override
  final String wireName = r'ProjectBudgetLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.category != null) {
      yield r'category';
      yield serializers.serialize(
        object.category,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'category':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.category = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetLineRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


