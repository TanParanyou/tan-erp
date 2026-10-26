//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/mrp_demand_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_run_request.g.dart';

/// MrpRunRequest
///
/// Properties:
/// * [asOfDate] 
/// * [purchaseLeadTimeDays] 
/// * [productionLeadTimeDays] 
/// * [includeOpenWorkOrders] 
/// * [demands] 
@BuiltValue()
abstract class MrpRunRequest implements Built<MrpRunRequest, MrpRunRequestBuilder> {
  @BuiltValueField(wireName: r'asOfDate')
  Date? get asOfDate;

  @BuiltValueField(wireName: r'purchaseLeadTimeDays')
  int? get purchaseLeadTimeDays;

  @BuiltValueField(wireName: r'productionLeadTimeDays')
  int? get productionLeadTimeDays;

  @BuiltValueField(wireName: r'includeOpenWorkOrders')
  bool? get includeOpenWorkOrders;

  @BuiltValueField(wireName: r'demands')
  BuiltList<MrpDemandRequest>? get demands;

  MrpRunRequest._();

  factory MrpRunRequest([void updates(MrpRunRequestBuilder b)]) = _$MrpRunRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpRunRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpRunRequest> get serializer => _$MrpRunRequestSerializer();
}

class _$MrpRunRequestSerializer implements PrimitiveSerializer<MrpRunRequest> {
  @override
  final Iterable<Type> types = const [MrpRunRequest, _$MrpRunRequest];

  @override
  final String wireName = r'MrpRunRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpRunRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.asOfDate != null) {
      yield r'asOfDate';
      yield serializers.serialize(
        object.asOfDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.purchaseLeadTimeDays != null) {
      yield r'purchaseLeadTimeDays';
      yield serializers.serialize(
        object.purchaseLeadTimeDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.productionLeadTimeDays != null) {
      yield r'productionLeadTimeDays';
      yield serializers.serialize(
        object.productionLeadTimeDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.includeOpenWorkOrders != null) {
      yield r'includeOpenWorkOrders';
      yield serializers.serialize(
        object.includeOpenWorkOrders,
        specifiedType: const FullType(bool),
      );
    }
    if (object.demands != null) {
      yield r'demands';
      yield serializers.serialize(
        object.demands,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MrpDemandRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpRunRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpRunRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'asOfDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.asOfDate = valueDes;
          break;
        case r'purchaseLeadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.purchaseLeadTimeDays = valueDes;
          break;
        case r'productionLeadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.productionLeadTimeDays = valueDes;
          break;
        case r'includeOpenWorkOrders':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.includeOpenWorkOrders = valueDes;
          break;
        case r'demands':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MrpDemandRequest)]),
          ) as BuiltList<MrpDemandRequest>?;
          if (valueDes == null) continue;
          result.demands.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpRunRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpRunRequestBuilder();
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


