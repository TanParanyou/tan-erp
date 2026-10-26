//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_project_change_order_request.g.dart';

/// CreateProjectChangeOrderRequest
///
/// Properties:
/// * [title] 
/// * [reason] 
/// * [budgetDelta] 
/// * [contractDelta] 
@BuiltValue()
abstract class CreateProjectChangeOrderRequest implements Built<CreateProjectChangeOrderRequest, CreateProjectChangeOrderRequestBuilder> {
  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'budgetDelta')
  double? get budgetDelta;

  @BuiltValueField(wireName: r'contractDelta')
  double? get contractDelta;

  CreateProjectChangeOrderRequest._();

  factory CreateProjectChangeOrderRequest([void updates(CreateProjectChangeOrderRequestBuilder b)]) = _$CreateProjectChangeOrderRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateProjectChangeOrderRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateProjectChangeOrderRequest> get serializer => _$CreateProjectChangeOrderRequestSerializer();
}

class _$CreateProjectChangeOrderRequestSerializer implements PrimitiveSerializer<CreateProjectChangeOrderRequest> {
  @override
  final Iterable<Type> types = const [CreateProjectChangeOrderRequest, _$CreateProjectChangeOrderRequest];

  @override
  final String wireName = r'CreateProjectChangeOrderRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateProjectChangeOrderRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.budgetDelta != null) {
      yield r'budgetDelta';
      yield serializers.serialize(
        object.budgetDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.contractDelta != null) {
      yield r'contractDelta';
      yield serializers.serialize(
        object.contractDelta,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateProjectChangeOrderRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateProjectChangeOrderRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'budgetDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.budgetDelta = valueDes;
          break;
        case r'contractDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractDelta = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateProjectChangeOrderRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateProjectChangeOrderRequestBuilder();
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


