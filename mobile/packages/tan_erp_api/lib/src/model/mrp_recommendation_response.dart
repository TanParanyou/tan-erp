//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/mrp_person_response.dart';
import 'package:tan_erp_api/src/model/mrp_item_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/mrp_reason_response.dart';
import 'package:tan_erp_api/src/model/mrp_converted_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_recommendation_response.g.dart';

/// MrpRecommendationResponse
///
/// Properties:
/// * [id] 
/// * [lineNo] 
/// * [item] 
/// * [action] 
/// * [quantity] 
/// * [needBy] 
/// * [orderBy] 
/// * [level] 
/// * [grossRequirement] 
/// * [stockUsed] 
/// * [scheduledReceiptsUsed] 
/// * [reasons] 
/// * [status] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [converted] 
/// * [rowVersion] 
@BuiltValue()
abstract class MrpRecommendationResponse implements Built<MrpRecommendationResponse, MrpRecommendationResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'lineNo')
  int? get lineNo;

  @BuiltValueField(wireName: r'item')
  MrpItemResponse? get item;

  @BuiltValueField(wireName: r'action')
  String? get action;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'needBy')
  Date? get needBy;

  @BuiltValueField(wireName: r'orderBy')
  Date? get orderBy;

  @BuiltValueField(wireName: r'level')
  int? get level;

  @BuiltValueField(wireName: r'grossRequirement')
  double? get grossRequirement;

  @BuiltValueField(wireName: r'stockUsed')
  double? get stockUsed;

  @BuiltValueField(wireName: r'scheduledReceiptsUsed')
  double? get scheduledReceiptsUsed;

  @BuiltValueField(wireName: r'reasons')
  BuiltList<MrpReasonResponse>? get reasons;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'decidedBy')
  MrpPersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'converted')
  MrpConvertedResponse? get converted;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  MrpRecommendationResponse._();

  factory MrpRecommendationResponse([void updates(MrpRecommendationResponseBuilder b)]) = _$MrpRecommendationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpRecommendationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpRecommendationResponse> get serializer => _$MrpRecommendationResponseSerializer();
}

class _$MrpRecommendationResponseSerializer implements PrimitiveSerializer<MrpRecommendationResponse> {
  @override
  final Iterable<Type> types = const [MrpRecommendationResponse, _$MrpRecommendationResponse];

  @override
  final String wireName = r'MrpRecommendationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpRecommendationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.lineNo != null) {
      yield r'lineNo';
      yield serializers.serialize(
        object.lineNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(MrpItemResponse),
      );
    }
    if (object.action != null) {
      yield r'action';
      yield serializers.serialize(
        object.action,
        specifiedType: const FullType.nullable(String),
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
    if (object.orderBy != null) {
      yield r'orderBy';
      yield serializers.serialize(
        object.orderBy,
        specifiedType: const FullType(Date),
      );
    }
    if (object.level != null) {
      yield r'level';
      yield serializers.serialize(
        object.level,
        specifiedType: const FullType(int),
      );
    }
    if (object.grossRequirement != null) {
      yield r'grossRequirement';
      yield serializers.serialize(
        object.grossRequirement,
        specifiedType: const FullType(double),
      );
    }
    if (object.stockUsed != null) {
      yield r'stockUsed';
      yield serializers.serialize(
        object.stockUsed,
        specifiedType: const FullType(double),
      );
    }
    if (object.scheduledReceiptsUsed != null) {
      yield r'scheduledReceiptsUsed';
      yield serializers.serialize(
        object.scheduledReceiptsUsed,
        specifiedType: const FullType(double),
      );
    }
    if (object.reasons != null) {
      yield r'reasons';
      yield serializers.serialize(
        object.reasons,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MrpReasonResponse)]),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(MrpPersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.converted != null) {
      yield r'converted';
      yield serializers.serialize(
        object.converted,
        specifiedType: const FullType(MrpConvertedResponse),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpRecommendationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpRecommendationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'lineNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lineNo = valueDes;
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpItemResponse),
          ) as MrpItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'action':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.action = valueDes;
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
        case r'orderBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.orderBy = valueDes;
          break;
        case r'level':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.level = valueDes;
          break;
        case r'grossRequirement':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grossRequirement = valueDes;
          break;
        case r'stockUsed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.stockUsed = valueDes;
          break;
        case r'scheduledReceiptsUsed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.scheduledReceiptsUsed = valueDes;
          break;
        case r'reasons':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MrpReasonResponse)]),
          ) as BuiltList<MrpReasonResponse>?;
          if (valueDes == null) continue;
          result.reasons.replace(valueDes);
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpPersonResponse),
          ) as MrpPersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'converted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpConvertedResponse),
          ) as MrpConvertedResponse?;
          if (valueDes == null) continue;
          result.converted.replace(valueDes);
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpRecommendationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpRecommendationResponseBuilder();
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


