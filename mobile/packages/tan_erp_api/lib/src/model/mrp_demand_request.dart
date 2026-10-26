//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_demand_request.g.dart';

/// MrpDemandRequest
///
/// Properties:
/// * [itemId] 
/// * [quantity] 
/// * [needBy] 
/// * [reference] 
@BuiltValue()
abstract class MrpDemandRequest implements Built<MrpDemandRequest, MrpDemandRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'needBy')
  Date? get needBy;

  @BuiltValueField(wireName: r'reference')
  String? get reference;

  MrpDemandRequest._();

  factory MrpDemandRequest([void updates(MrpDemandRequestBuilder b)]) = _$MrpDemandRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpDemandRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpDemandRequest> get serializer => _$MrpDemandRequestSerializer();
}

class _$MrpDemandRequestSerializer implements PrimitiveSerializer<MrpDemandRequest> {
  @override
  final Iterable<Type> types = const [MrpDemandRequest, _$MrpDemandRequest];

  @override
  final String wireName = r'MrpDemandRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpDemandRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.needBy != null) {
      yield r'needBy';
      yield serializers.serialize(
        object.needBy,
        specifiedType: const FullType(Date),
      );
    }
    if (object.reference != null) {
      yield r'reference';
      yield serializers.serialize(
        object.reference,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpDemandRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpDemandRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'needBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.needBy = valueDes;
          break;
        case r'reference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reference = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpDemandRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpDemandRequestBuilder();
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


