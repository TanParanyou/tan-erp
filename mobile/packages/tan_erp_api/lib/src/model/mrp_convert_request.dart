//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_convert_request.g.dart';

/// MrpConvertRequest
///
/// Properties:
/// * [supplierId] 
/// * [unitPrice] 
/// * [warehouseId] 
@BuiltValue()
abstract class MrpConvertRequest implements Built<MrpConvertRequest, MrpConvertRequestBuilder> {
  @BuiltValueField(wireName: r'supplierId')
  String? get supplierId;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  MrpConvertRequest._();

  factory MrpConvertRequest([void updates(MrpConvertRequestBuilder b)]) = _$MrpConvertRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpConvertRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpConvertRequest> get serializer => _$MrpConvertRequestSerializer();
}

class _$MrpConvertRequestSerializer implements PrimitiveSerializer<MrpConvertRequest> {
  @override
  final Iterable<Type> types = const [MrpConvertRequest, _$MrpConvertRequest];

  @override
  final String wireName = r'MrpConvertRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpConvertRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.supplierId != null) {
      yield r'supplierId';
      yield serializers.serialize(
        object.supplierId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitPrice != null) {
      yield r'unitPrice';
      yield serializers.serialize(
        object.unitPrice,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpConvertRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpConvertRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'supplierId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierId = valueDes;
          break;
        case r'unitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitPrice = valueDes;
          break;
        case r'warehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpConvertRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpConvertRequestBuilder();
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


