//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_item_barcode_request.g.dart';

/// CreateItemBarcodeRequest
///
/// Properties:
/// * [identifierType] 
/// * [value] 
/// * [unitId] 
/// * [quantityInBaseUnit] 
/// * [packagingLevel] 
/// * [isPrimary] 
@BuiltValue()
abstract class CreateItemBarcodeRequest implements Built<CreateItemBarcodeRequest, CreateItemBarcodeRequestBuilder> {
  @BuiltValueField(wireName: r'identifierType')
  String get identifierType;

  @BuiltValueField(wireName: r'value')
  String get value;

  @BuiltValueField(wireName: r'unitId')
  String get unitId;

  @BuiltValueField(wireName: r'quantityInBaseUnit')
  String get quantityInBaseUnit;

  @BuiltValueField(wireName: r'packagingLevel')
  String get packagingLevel;

  @BuiltValueField(wireName: r'isPrimary')
  bool? get isPrimary;

  CreateItemBarcodeRequest._();

  factory CreateItemBarcodeRequest([void updates(CreateItemBarcodeRequestBuilder b)]) = _$CreateItemBarcodeRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateItemBarcodeRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateItemBarcodeRequest> get serializer => _$CreateItemBarcodeRequestSerializer();
}

class _$CreateItemBarcodeRequestSerializer implements PrimitiveSerializer<CreateItemBarcodeRequest> {
  @override
  final Iterable<Type> types = const [CreateItemBarcodeRequest, _$CreateItemBarcodeRequest];

  @override
  final String wireName = r'CreateItemBarcodeRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateItemBarcodeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'identifierType';
    yield serializers.serialize(
      object.identifierType,
      specifiedType: const FullType(String),
    );
    yield r'value';
    yield serializers.serialize(
      object.value,
      specifiedType: const FullType(String),
    );
    yield r'unitId';
    yield serializers.serialize(
      object.unitId,
      specifiedType: const FullType(String),
    );
    yield r'quantityInBaseUnit';
    yield serializers.serialize(
      object.quantityInBaseUnit,
      specifiedType: const FullType(String),
    );
    yield r'packagingLevel';
    yield serializers.serialize(
      object.packagingLevel,
      specifiedType: const FullType(String),
    );
    if (object.isPrimary != null) {
      yield r'isPrimary';
      yield serializers.serialize(
        object.isPrimary,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateItemBarcodeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateItemBarcodeRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'identifierType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.identifierType = valueDes;
          break;
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.value = valueDes;
          break;
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.unitId = valueDes;
          break;
        case r'quantityInBaseUnit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.quantityInBaseUnit = valueDes;
          break;
        case r'packagingLevel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.packagingLevel = valueDes;
          break;
        case r'isPrimary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPrimary = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateItemBarcodeRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateItemBarcodeRequestBuilder();
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


