//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_work_item_item_response.g.dart';

/// EstimateWorkItemItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [nameTh] 
/// * [nameEn] 
@BuiltValue()
abstract class EstimateWorkItemItemResponse implements Built<EstimateWorkItemItemResponse, EstimateWorkItemItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String get id;

  @BuiltValueField(wireName: r'code')
  String get code;

  @BuiltValueField(wireName: r'nameTh')
  String get nameTh;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  EstimateWorkItemItemResponse._();

  factory EstimateWorkItemItemResponse([void updates(EstimateWorkItemItemResponseBuilder b)]) = _$EstimateWorkItemItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateWorkItemItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateWorkItemItemResponse> get serializer => _$EstimateWorkItemItemResponseSerializer();
}

class _$EstimateWorkItemItemResponseSerializer implements PrimitiveSerializer<EstimateWorkItemItemResponse> {
  @override
  final Iterable<Type> types = const [EstimateWorkItemItemResponse, _$EstimateWorkItemItemResponse];

  @override
  final String wireName = r'EstimateWorkItemItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateWorkItemItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'id';
    yield serializers.serialize(
      object.id,
      specifiedType: const FullType(String),
    );
    yield r'code';
    yield serializers.serialize(
      object.code,
      specifiedType: const FullType(String),
    );
    yield r'nameTh';
    yield serializers.serialize(
      object.nameTh,
      specifiedType: const FullType(String),
    );
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateWorkItemItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateWorkItemItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.code = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.nameTh = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateWorkItemItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateWorkItemItemResponseBuilder();
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


